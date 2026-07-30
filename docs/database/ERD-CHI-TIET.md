# GiapTech.BlouseHiding — ERD chi tiết

> Cụ thể hóa mô hình dữ liệu cốt lõi của [`../nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](../nghiep-vu/PHAN-TICH-NGHIEP-VU.md)
> và mục 1 (Luồng nghiệp vụ) của [`../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md).
> Kiểu dữ liệu viết theo PostgreSQL. Mỗi bảng đều có `id UUID PK`, `created_at`, `updated_at` trừ khi ghi chú khác.
> Quy ước migration: [`QUY-UOC-MIGRATION.md`](./QUY-UOC-MIGRATION.md).

---

## 0. Công nghệ CSDL & lưu trữ

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| CSDL chính | **PostgreSQL 16+** | Mạnh về JSONB (lưu `data_json` của CV Builder, `payload` thông báo), full-text search sẵn có cho MVP |
| Full-text MVP | Extension **`pg_trgm`** + `tsvector` trên `jobs`, `candidate_profiles` | Đủ dùng cho tìm kiếm ở Giai đoạn 1, tránh vận hành OpenSearch quá sớm |
| Search nâng cao (GĐ2) | **OpenSearch** (thay vì Elasticsearch) | License Apache 2.0 rõ ràng hơn Elastic License, tương thích API Elasticsearch cũ |
| Cache | **Redis 7+** qua `StackExchange.Redis` | Cache kết quả tìm kiếm, session SignalR backplane, rate-limit counter |
| Object storage | **MinIO** tự host (API tương thích S3) | Ảnh CCHN, giấy phép, CV PDF, logo — không lưu trong Postgres |
| Backup | `pg_dump`/`pg_basebackup` định kỳ (cron) + WAL archiving, đẩy bản backup ra **ngoài VPS chính** | Tự host không có snapshot managed — **bắt buộc** có bản sao ngoài máy chủ chính + diễn tập khôi phục định kỳ. Chi tiết quy trình: [`../ha-tang/VAN-HANH-RUNBOOK.md`](../ha-tang/VAN-HANH-RUNBOOK.md) |

---

## 1. Sơ đồ quan hệ theo nhóm (Mermaid ER)

### 1.1 Identity & Hồ sơ ứng viên

```mermaid
erDiagram
    USER ||--o| CANDIDATE_PROFILE : "1-1"
    CANDIDATE_PROFILE ||--o{ LICENSE : has
    CANDIDATE_PROFILE ||--o{ PROFILE_SPECIALTY : has
    CANDIDATE_PROFILE ||--o{ EXPERIENCE : has
    CANDIDATE_PROFILE ||--o{ EDUCATION : has
    CANDIDATE_PROFILE ||--o{ CONTINUING_CERTIFICATE : has
    CANDIDATE_PROFILE ||--o{ CV : has
    SPECIALTY ||--o{ PROFILE_SPECIALTY : "referenced by"

    USER {
        uuid id PK
        string email UK
        string phone UK
        string password_hash
        enum role
        enum status
        timestamp email_verified_at
        timestamp phone_verified_at
    }
    CANDIDATE_PROFILE {
        uuid id PK
        uuid user_id FK
        string full_name
        date dob
        enum gender
        string headline
        text summary
        int completion_pct
    }
    LICENSE {
        uuid id PK
        uuid profile_id FK
        string license_no UK
        string issued_by
        string scope
        date issued_at
        date expired_at
        enum verify_status
        uuid verified_by FK
        text reject_reason
        string document_url
    }
    SPECIALTY {
        uuid id PK
        string code UK
        string name
        uuid parent_id FK
    }
    PROFILE_SPECIALTY {
        uuid id PK
        uuid profile_id FK
        uuid specialty_id FK
        enum level
    }
```

### 1.2 Employer & Job

```mermaid
erDiagram
    ORGANIZATION ||--o{ EMPLOYER_MEMBER : has
    USER ||--o{ EMPLOYER_MEMBER : "is member"
    ORGANIZATION ||--o{ JOB : posts
    ORGANIZATION ||--o{ ORGANIZATION_DOCUMENT : has
    JOB ||--o{ JOB_PURCHASE : "purchased with"
    JOB_PACKAGE ||--o{ JOB_PURCHASE : "used in"
    JOB }o--|| SPECIALTY : "belongs to"
    JOB }o--|| LOCATION : "located at"

    ORGANIZATION {
        uuid id PK
        string name
        enum org_type
        string license_no
        enum size
        enum verify_status
        uuid verified_by FK
        text reject_reason
    }
    EMPLOYER_MEMBER {
        uuid id PK
        uuid org_id FK
        uuid user_id FK
        enum member_role
        uuid invited_by FK
        timestamp joined_at
    }
    JOB {
        uuid id PK
        uuid org_id FK
        string title
        uuid specialty_id FK
        enum employment_type
        int salary_min
        int salary_max
        bool salary_negotiable
        uuid location_id FK
        bool required_license
        int min_experience_years
        text description
        text requirements
        text benefits
        enum status
        text reject_reason
        timestamp published_at
        timestamp expires_at
        uuid created_by FK
    }
    JOB_PACKAGE {
        uuid id PK
        enum tier
        string name
        int duration_days
        numeric price
        int max_active_jobs
        jsonb perks
    }
    JOB_PURCHASE {
        uuid id PK
        uuid job_id FK
        uuid package_id FK
        uuid org_id FK
        uuid payment_id FK
        timestamp purchased_at
        timestamp expires_at
    }
```

### 1.3 Application, Credit & Messaging

```mermaid
erDiagram
    JOB ||--o{ APPLICATION : receives
    CANDIDATE_PROFILE ||--o{ APPLICATION : submits
    APPLICATION ||--o{ APPLICATION_NOTE : has
    APPLICATION ||--o{ APPLICATION_STAGE_HISTORY : has
    ORGANIZATION ||--|| CREDIT_WALLET : owns
    CREDIT_WALLET ||--o{ CREDIT_TRANSACTION : records
    ORGANIZATION ||--o{ PROFILE_UNLOCK : unlocks
    CANDIDATE_PROFILE ||--o{ PROFILE_UNLOCK : "unlocked by"
    CANDIDATE_PROFILE ||--o{ CONVERSATION : participates
    ORGANIZATION ||--o{ CONVERSATION : participates
    CONVERSATION ||--o{ MESSAGE : contains

    APPLICATION {
        uuid id PK
        uuid job_id FK
        uuid candidate_id FK
        uuid cv_id FK
        text cover_letter
        enum stage
        int score
        text rejected_reason
        timestamp applied_at
    }
    CREDIT_WALLET {
        uuid id PK
        uuid org_id FK
        int balance
    }
    CREDIT_TRANSACTION {
        uuid id PK
        uuid wallet_id FK
        int amount
        enum reason
        uuid reference_id
    }
    PROFILE_UNLOCK {
        uuid id PK
        uuid org_id FK
        uuid candidate_id FK
        int credit_cost
        uuid unlocked_by FK
        timestamp unlocked_at
    }
    CONVERSATION {
        uuid id PK
        uuid job_id FK
        uuid candidate_id FK
        uuid org_id FK
        timestamp last_message_at
    }
    MESSAGE {
        uuid id PK
        uuid conversation_id FK
        uuid sender_user_id FK
        text content
        string attachment_url
        timestamp sent_at
        timestamp read_at
    }
```

### 1.4 Vận hành, Events & Content (Giai đoạn 3)

```mermaid
erDiagram
    EVENT ||--o{ EVENT_REGISTRATION : has
    USER ||--o{ EVENT_REGISTRATION : registers
    ORGANIZATION ||--o{ EVENT : organizes
    ORGANIZATION ||--o{ ORGANIZATION_REVIEW : "reviewed"
    CANDIDATE_PROFILE ||--o{ ORGANIZATION_REVIEW : writes
    USER ||--o{ REPORT : files
    USER ||--o{ AUDIT_LOG : performs

    EVENT {
        uuid id PK
        enum event_type
        string title
        uuid organizer_org_id FK
        timestamp start_at
        timestamp end_at
        string location
        bool is_online
        enum status
    }
    ORGANIZATION_REVIEW {
        uuid id PK
        uuid org_id FK
        uuid candidate_id FK
        int rating
        text comment
        enum status
    }
    REPORT {
        uuid id PK
        uuid reporter_user_id FK
        enum target_type
        uuid target_id
        text reason
        enum status
        uuid resolved_by FK
    }
    AUDIT_LOG {
        uuid id PK
        uuid actor_user_id FK
        string action
        string entity_type
        uuid entity_id
        jsonb before_state
        jsonb after_state
        string ip_address
    }
```

---

## 2. Định nghĩa bảng đầy đủ

### 2.1 Identity

**`users`**
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| email | varchar(255) | UNIQUE, NOT NULL | |
| phone | varchar(20) | UNIQUE | |
| password_hash | varchar(255) | NOT NULL | bcrypt/argon2 |
| role | enum | NOT NULL | `candidate`, `employer` (dùng trang **Admin**), `admin`/`moderator` (đội **Vận hành** — xem `../nghiep-vu/PHAN-TICH-NGHIEP-VU.md` mục 2) |
| status | enum | NOT NULL DEFAULT `active` | `active`, `suspended`, `deleted` |
| locale | enum | NOT NULL DEFAULT `vi` | `vi`, `en`, `ja`, `zh`, `ko`, `es` — ngôn ngữ giao diện đã chọn, dùng để gửi email/thông báo đúng ngôn ngữ (xem [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md)) |
| email_verified_at | timestamp | nullable | |
| phone_verified_at | timestamp | nullable | |
| created_at / updated_at | timestamp | NOT NULL | |

**`refresh_tokens`**
| Cột | Kiểu | Ràng buộc |
|---|---|---|
| id | uuid | PK |
| user_id | uuid | FK → users |
| token_hash | varchar(255) | NOT NULL |
| expires_at | timestamp | NOT NULL |
| revoked_at | timestamp | nullable |

### 2.2 Hồ sơ ứng viên

**`candidate_profiles`** (1-1 với `users`)
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| user_id | uuid | FK UNIQUE → users | |
| full_name | varchar(255) | NOT NULL | |
| dob | date | nullable | |
| gender | enum | nullable | |
| avatar_url | text | nullable | |
| headline | varchar(255) | nullable | vd. "Điều dưỡng ICU 5 năm kinh nghiệm" |
| summary | text | nullable | |
| address, city_id | varchar / FK | nullable | |
| completion_pct | int | DEFAULT 0 | tính toán, hiển thị thanh tiến độ hồ sơ |

**`licenses`** (chứng chỉ hành nghề — **bảng lõi tạo tin cậy**)
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| profile_id | uuid | FK → candidate_profiles | |
| license_no | varchar(100) | NOT NULL | số CCHN |
| issued_by | varchar(255) | NOT NULL | Sở Y tế / Bộ Y tế cấp |
| scope | text | nullable | phạm vi hoạt động chuyên môn |
| issued_at | date | NOT NULL | |
| expired_at | date | nullable | NULL = không thời hạn |
| verify_status | enum | NOT NULL DEFAULT `pending` | `pending`, `verified`, `rejected`, `expired` |
| verified_by | uuid | FK → users, nullable | đội Vận hành duyệt |
| verified_at | timestamp | nullable | |
| reject_reason | text | nullable | |
| document_url | text | NOT NULL | ảnh/scan chứng chỉ |

*Ràng buộc nghiệp vụ:* `expired_at < now()` → job tự động chuyển `verify_status = expired` (xử lý ở worker định kỳ, không xóa dữ liệu).

**`specialties`** (danh mục chuyên khoa, cây phân cấp)
| Cột | Kiểu | Ràng buộc |
|---|---|---|
| id | uuid | PK |
| code | varchar(50) | UNIQUE |
| name | varchar(255) | NOT NULL — tên tiếng Việt, dùng làm giá trị dự phòng khi thiếu bản dịch |
| parent_id | uuid | FK → specialties, nullable |

**`specialty_translations`** (bản dịch tên chuyên khoa — xem [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md))
| Cột | Kiểu | Ràng buộc |
|---|---|---|
| id | uuid | PK |
| specialty_id | uuid | FK → specialties |
| locale | varchar(5) | `en`, `ja`, `zh`, `ko`, `es` (không lưu `vi` — đã có ở `specialties.name`) |
| name | varchar(255) | NOT NULL |
| UNIQUE(specialty_id, locale) | | |

**`profile_specialties`** (n-n giữa hồ sơ và chuyên khoa)
| Cột | Kiểu | Ràng buộc |
|---|---|---|
| id | uuid | PK |
| profile_id | uuid | FK → candidate_profiles |
| specialty_id | uuid | FK → specialties |
| level | enum | `junior`, `mid`, `senior`, `expert` |

**`experiences`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid | PK |
| profile_id | uuid | FK |
| organization_name | varchar(255) | |
| position | varchar(255) | |
| tier | enum | `trung_uong`, `tinh`, `huyen`, `tu_nhan` |
| from_date / to_date | date | to_date NULL = đang làm |
| description | text | |

**`educations`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| profile_id | uuid FK |
| school_name | varchar(255) |
| degree | varchar(100) |
| major | varchar(255) |
| from_year / to_year | int |

**`continuing_certificates`** (CME)
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| profile_id | uuid FK |
| name | varchar(255) |
| issuer | varchar(255) |
| issued_at / expired_at | date |
| document_url | text |

**`cvs`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid | PK |
| profile_id | uuid | FK |
| template_id | varchar(50) | nullable nếu upload |
| is_primary | bool | DEFAULT false |
| file_url | text | nullable |
| data_json | jsonb | nullable, dữ liệu CV Builder |

### 2.3 Cơ sở y tế (Employer)

**`organizations`**
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| name | varchar(255) | NOT NULL | |
| org_type | enum | NOT NULL | `benh_vien_cong`, `benh_vien_tu`, `phong_kham`, `nha_thuoc`, `cong_ty_duoc`, `phong_lab` |
| license_no | varchar(100) | nullable | số giấy phép hoạt động |
| size | enum | nullable | `<50`, `50-200`, `200-1000`, `>1000` |
| description | text | nullable | |
| logo_url / cover_url | text | nullable | |
| address, city_id | varchar / FK | | |
| verify_status | enum | NOT NULL DEFAULT `pending` | `pending`, `verified`, `rejected`, `suspended` (rút xác thực sau khi đã verified — xem mục 4 điểm 6) |
| verified_by | uuid | FK → users, nullable | |
| reject_reason | text | nullable | |

**`organization_documents`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| org_id | uuid FK |
| doc_type | varchar(100) |
| file_url | text |

**`employer_members`**
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| org_id | uuid | FK → organizations | |
| user_id | uuid | FK → users | |
| member_role | enum | NOT NULL | `owner`, `hr_manager`, `hr_member` |
| invited_by | uuid | FK → users, nullable | |
| joined_at | timestamp | | |
| UNIQUE(org_id, user_id) | | | |

**`organization_invitations`** (mời thành viên HR — kể cả người **chưa có tài khoản**)
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| org_id | uuid | FK → organizations | |
| email | varchar(255) | NOT NULL | email người được mời — chưa cần tồn tại `users` |
| invited_role | enum | NOT NULL | `hr_manager`, `hr_member` (không mời thêm `owner` qua đây) |
| invited_by | uuid | FK → users | |
| token_hash | varchar(255) | NOT NULL | token xác nhận gửi qua email, hash lưu DB |
| expires_at | timestamp | NOT NULL | vd. 7 ngày |
| accepted_at | timestamp | nullable | NULL = chưa chấp nhận |

*Luồng:* mời → tạo dòng này + gửi email link chứa token → người được mời bấm link, nếu **chưa có tài
khoản** thì đăng ký trước (email khớp lời mời) → chấp nhận → hệ thống tạo `employer_members` thật và
set `accepted_at`. Không tạo `employer_members` với `user_id` rỗng ở bất kỳ bước nào.

### 2.4 Danh mục dùng chung

**`locations`** (cây: tỉnh/thành → quận/huyện)
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| name | varchar(255) — tên tiếng Việt, giá trị dự phòng khi thiếu bản dịch |
| parent_id | uuid FK nullable |

**`location_translations`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| location_id | uuid FK → locations |
| locale | varchar(5) — `en`, `ja`, `zh`, `ko`, `es` |
| name | varchar(255) NOT NULL |
| UNIQUE(location_id, locale) | |

### 2.5 Tin tuyển dụng & monetization

**`jobs`**
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| org_id | uuid | FK → organizations | |
| title | varchar(255) | NOT NULL | |
| specialty_id | uuid | FK → specialties | |
| employment_type | enum | NOT NULL | `full_time`, `part_time`, `truc_ca`, `locum`, `ctv` |
| salary_min / salary_max | int | nullable | đơn vị VND |
| salary_negotiable | bool | DEFAULT false | |
| location_id | uuid | FK → locations | |
| address_detail | varchar(255) | nullable | |
| required_license | bool | DEFAULT true | |
| min_experience_years | int | DEFAULT 0 | |
| description / requirements / benefits | text | | ngôn ngữ tự do theo tác giả nhập — **không dịch**, hiển thị nguyên văn bất kể locale người xem (ADR-0006) |
| status | enum | NOT NULL DEFAULT `draft` | `draft`, `pending_payment`, `pending`, `published`, `rejected`, `expired`, `closed`, `suspended` — xem mục 4 điểm 6-7 |
| reject_reason | text | nullable | |
| published_at / expires_at | timestamp | nullable | |
| created_by | uuid | FK → users | |

**`job_packages`** (danh mục gói — dữ liệu cấu hình, không phải giao dịch)
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| tier | enum | `free`, `eco`, `pro`, `max` |
| name | varchar(100) | tên tiếng Việt, giá trị dự phòng khi thiếu bản dịch |
| duration_days | int | vd. 14 |
| price | numeric(12,2) | VND |
| max_active_jobs | int | giới hạn cho gói free |
| perks | jsonb | vd. `{"pin_top": true, "highlight": true}` |

**`job_package_translations`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| job_package_id | uuid FK → job_packages | |
| locale | varchar(5) | `en`, `ja`, `zh`, `ko`, `es` |
| name | varchar(100) | NOT NULL |
| UNIQUE(job_package_id, locale) | | |

**`job_purchases`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| job_id | uuid FK → jobs | |
| package_id | uuid FK → job_packages | |
| org_id | uuid FK → organizations | |
| payment_id | uuid FK → payments, nullable | null nếu gói free |
| purchased_at | timestamp | |
| expires_at | timestamp | |

**`payments`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| org_id | uuid FK | |
| type | enum | `job_package`, `credit_topup` |
| amount | numeric(12,2) | |
| provider | enum | `manual_transfer` (MVP — xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md)), `vnpay`, `momo`, `zalopay` (dự phòng khi có cổng tự động) |
| reference_code | varchar(20) UNIQUE | **Bắt buộc với `provider = manual_transfer`** — mã hiển thị cho NTD ghi vào nội dung chuyển khoản, dùng để đối soát thủ công. Sinh ngẫu nhiên, dễ đọc (vd `PAY-7F3K2Q`) |
| provider_txn_id | varchar(255) | nullable — chỉ có khi `provider` là cổng tự động |
| status | enum | `pending`, `success`, `failed` |
| confirmed_by | uuid FK → users, nullable | nhân viên Vận hành xác nhận thủ công (NULL nếu do webhook tự động xác nhận sau này) |

### 2.6 Ứng tuyển & ATS

**`applications`**
| Cột | Kiểu | Ràng buộc | Ghi chú |
|---|---|---|---|
| id | uuid | PK | |
| job_id | uuid | FK → jobs | |
| candidate_id | uuid | FK → candidate_profiles | |
| cv_id | uuid | FK → cvs | trỏ tới CV gốc — chỉ để truy vết, **không dùng để hiển thị** (xem `cv_snapshot`) |
| cv_snapshot | jsonb | NOT NULL | **Bản chụp CV tại thời điểm ứng tuyển** (nội dung CV Builder hoặc URL file lúc đó). NTD luôn xem bản này, không phải bản `cv_id` hiện tại — tránh việc ứng viên sửa CV sau khi nộp làm thay đổi ngược những gì NTD đã thấy/đánh giá |
| cover_letter | text | nullable | |
| stage | enum | NOT NULL DEFAULT `new` | `new`, `reviewing`, `shortlisted`, `interview`, `offer`, `hired`, `rejected` |
| score | int | nullable | CV Scoring — tính **1 lần** ngay khi tạo `application` (dựa trên `cv_snapshot` + CCHN + chuyên khoa lúc đó), không tính lại khi hồ sơ gốc thay đổi sau này; HR có thể ghi đè thủ công |
| rejected_reason | text | nullable | khuyến khích nhập khi từ chối (không bắt buộc NOT NULL — HR có thể từ chối hàng loạt) |
| applied_at | timestamp | | |
| UNIQUE(job_id, candidate_id) | | | 1 ứng viên/1 tin chỉ nộp 1 lần — không áp dụng khi tin được tạo mới do "gia hạn" (xem mục 4 điểm 7), vì đó là `job_id` khác |

**`application_notes`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| application_id | uuid FK |
| author_user_id | uuid FK |
| note_text | text |

**`application_stage_history`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| application_id | uuid FK |
| from_stage / to_stage | enum |
| changed_by | uuid FK |
| changed_at | timestamp |

### 2.7 Credit & Profile Unlock

**`credit_wallets`** (1-1 với organizations)
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| org_id | uuid FK UNIQUE |
| balance | int DEFAULT 0 |

**`credit_transactions`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| wallet_id | uuid FK | |
| amount | int | dương = nạp, âm = trừ |
| reason | enum | `purchase`, `unlock_profile`, `refund`, `bonus` |
| reference_id | uuid | trỏ tới `payments.id` hoặc `profile_unlocks.id` tùy `reason` |
| created_by | uuid FK → users, nullable | NULL nếu hệ thống tự động trừ (vd `unlock_profile`); có giá trị khi Vận hành thao tác thủ công (`refund`, `bonus`) |

*`refund` chỉ tạo qua* `POST /ops/organizations/{id}/credit-refund` (xem
[`../backend/API-DESIGN.md`](../backend/API-DESIGN.md) mục 11) — dùng khi có tranh chấp (vd hồ sơ mở
ra sai/trùng, lỗi hệ thống trừ nhầm). Không có luồng tự động hoàn credit.

**`profile_unlocks`**
| Cột | Kiểu | Ràng buộc |
|---|---|---|
| id | uuid | PK |
| org_id | uuid | FK |
| candidate_id | uuid | FK |
| credit_cost | int | |
| unlocked_by | uuid | FK → users |
| unlocked_at | timestamp | |
| UNIQUE(org_id, candidate_id) | | mở 1 lần, xem lại không mất thêm credit |

### 2.8 Nhắn tin & thông báo

**`conversations`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| job_id | uuid FK nullable |
| candidate_id | uuid FK |
| org_id | uuid FK |
| last_message_at | timestamp |

**`messages`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| conversation_id | uuid FK |
| sender_user_id | uuid FK |
| content | text |
| attachment_url | text nullable |
| sent_at | timestamp |
| read_at | timestamp nullable |

**`notifications`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| user_id | uuid FK | |
| type | varchar(100) | vd. `job_match`, `application_stage_changed` |
| title / body | varchar / text | |
| payload | jsonb | deep-link data |
| channel | enum | `in_app`, `email`, `sms`, `zalo` |
| read_at | timestamp nullable | |

### 2.9 Sự kiện, đánh giá, kiểm duyệt (đa số Giai đoạn 2–3)

**`events`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| event_type | enum | `hoi_thao`, `cme`, `workshop`, `hoc_bong`, `cuoc_thi` |
| title | varchar(255) | |
| organizer_org_id | uuid FK nullable | |
| start_at / end_at | timestamp | |
| location | varchar(255) | |
| is_online | bool | |
| status | enum | `draft`, `published`, `cancelled` |

**`event_registrations`**
| Cột | Kiểu |
|---|---|
| id | uuid PK |
| event_id | uuid FK |
| user_id | uuid FK |
| registered_at | timestamp |
| attended | bool DEFAULT false |

**`organization_reviews`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| org_id | uuid FK | |
| candidate_id | uuid FK | |
| rating | int | 1–5 |
| comment | text | |
| status | enum | `pending`, `approved`, `rejected` — kiểm duyệt trước khi hiển thị |

**`reports`**
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| reporter_user_id | uuid FK | |
| target_type | enum | `job`, `organization`, `profile`, `message` |
| target_id | uuid | |
| reason | text | |
| status | enum | `pending`, `resolved`, `dismissed` |
| resolved_by | uuid FK nullable | |

**`audit_logs`** (tuân thủ NĐ 13/2023 — bất biến, không update/delete)
| Cột | Kiểu | Ghi chú |
|---|---|---|
| id | uuid PK | |
| actor_user_id | uuid FK nullable | null nếu hệ thống tự động |
| action | varchar(100) | vd. `license.verify`, `profile.delete` |
| entity_type / entity_id | varchar / uuid | |
| before_state / after_state | jsonb nullable | |
| ip_address | inet | |
| created_at | timestamp | |

---

## 3. Chỉ mục (index) quan trọng cho hiệu năng

| Bảng | Index | Lý do |
|---|---|---|
| jobs | (status, specialty_id, location_id) | lọc tìm kiếm chính |
| jobs | (org_id, status) | dashboard NTD |
| applications | (job_id, stage) | ATS Kanban theo tin |
| applications | (candidate_id) | "việc đã ứng tuyển" |
| licenses | (verify_status) | hàng đợi kiểm duyệt |
| profile_unlocks | (org_id, candidate_id) UNIQUE | idempotent unlock |
| notifications | (user_id, read_at) | inbox chưa đọc |
| messages | (conversation_id, sent_at) | load hội thoại theo thời gian |
| payments | (reference_code) UNIQUE | Vận hành tra cứu đối soát chuyển khoản thủ công |
| organization_invitations | (email, accepted_at) | kiểm tra lời mời đang chờ khi user đăng ký |

> Ở Giai đoạn 2, tìm kiếm chuyển sang Elasticsearch — các cột lọc trên vẫn giữ ở Postgres làm nguồn sự thật (source of truth), Elasticsearch chỉ là index phái sinh.

---

## 4. Ràng buộc nghiệp vụ quan trọng cần enforce ở tầng Application (không chỉ DB)

1. `jobs.status = published` chỉ khi `organizations.verify_status = verified`.
2. `applications` chỉ tạo được khi `licenses.verify_status` của ứng viên **không bắt buộc phải `verified`**
   (theo quyết định ở mục 1.1 luồng nghiệp vụ — hồ sơ chưa xác thực vẫn ứng tuyển được), nhưng
   `jobs.required_license = true` thì UI phải cảnh báo rõ trước khi NTD xem xét.
3. `profile_unlocks` — trừ credit qua transaction DB (SERIALIZABLE hoặc row lock trên `credit_wallets`)
   để tránh race condition khi trừ đồng thời.
4. `licenses.expired_at` quá hạn → job định kỳ cập nhật `verify_status = expired`, không tự xóa liên kết.
5. Xóa tài khoản (NĐ 13/2023) → soft-delete `users.status = deleted` + anonymize PII, giữ lại
   `applications`/`audit_logs` ở dạng đã ẩn danh để không phá vỡ số liệu thống kê của NTD.
6. **`organizations.verify_status` chuyển từ `verified` sang bất kỳ giá trị khác (`rejected`/`suspended`)**
   → **tự động** (không phải thao tác thủ công riêng) chuyển toàn bộ `jobs.status = published` của tổ
   chức đó sang `suspended` trong cùng transaction. Tin `suspended` bị ẩn khỏi tìm kiếm/trang công khai
   nhưng **không xóa dữ liệu**; muốn hiện lại phải chờ tổ chức được `verified` lại **và** Vận hành duyệt
   lại từng tin thủ công (không tự động published lại). Quyết định tại mục thảo luận thiết kế.
7. **"Gia hạn" tin tuyển dụng luôn tạo `jobs` row MỚI** (sao chép nội dung, `expires_at` mới), **không**
   tái sử dụng `job_id` cũ. Nhờ vậy ràng buộc `UNIQUE(job_id, candidate_id)` ở `applications` không
   chặn ứng viên từng bị từ chối ứng tuyển lại ở đợt tuyển mới — vì đó là `job_id` khác. Tin cũ chuyển
   `status = closed` khi tin mới được tạo từ nó.
8. `applications.cv_snapshot` được ghi **1 lần duy nhất** lúc tạo application (copy từ CV đang chọn tại
   thời điểm đó) — sửa CV gốc (`cvs`) sau này **không** ảnh hưởng tới các application đã nộp trước đó.
   `applications.score` tính dựa trên `cv_snapshot`, cũng không tự tính lại khi hồ sơ gốc đổi.
9. Chuyển `jobs.status` sang `expired`/`closed`/`suspended` **không khóa thao tác ATS** — NTD vẫn xem,
   chuyển `stage`, ghi chú, chấm điểm các `applications` đã có của tin đó bình thường; chỉ tin công khai
   (search, trang chi tiết) bị ảnh hưởng.
10. Report được Vận hành xử lý với hành động "gỡ nội dung" → phải trigger đúng state change tương ứng
    của entity bị báo cáo trong cùng thao tác (vd tin → `closed`, tổ chức → `suspended` theo điểm 6),
    không phải 2 bước thủ công tách rời dễ quên.
11. Thiếu bản dịch ở `*_translations` cho 1 locale nào đó **không phải lỗi** — tầng Application phải tự
    lùi về cột `name` (tiếng Việt) của bảng gốc khi JOIN không tìm thấy dòng dịch tương ứng, không để
    frontend nhận chuỗi rỗng/null. Không dịch máy tự động để lấp khoảng trống (xem
    [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md)).
12. Danh mục ít thay đổi (`specialties`, `locations`, `job_packages` + bản dịch) nên **cache theo
    locale** ở tầng Redis/Application — tránh JOIN bảng dịch lặp lại ở mọi request danh mục.

---

## 5. Xem thêm

- Hợp đồng API dựa trên schema này: [`../backend/API-DESIGN.md`](../backend/API-DESIGN.md)
- Quy ước migration: [`QUY-UOC-MIGRATION.md`](./QUY-UOC-MIGRATION.md)
