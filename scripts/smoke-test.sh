#!/bin/bash
# ============================================================================
# Smoke test luồng nghiệp vụ chính — dùng ở bước "Verify chạy thật" của
# checklist chốt giai đoạn (docs/nghiep-vu/TIEN-DO-DU-AN.md mục 1).
#
# CÁCH DÙNG:
#   docker compose up -d                       # hạ tầng
#   cd api && dotnet run --project src/Web &   # API ở cổng 5256
#   bash scripts/smoke-test.sh
#
# Chạy toàn bộ luồng trên dữ liệu MỚI mỗi lần (email/tổ chức có timestamp) —
# mục đích là chứng minh người dùng mới đăng ký hôm nay đi hết được luồng, không
# phụ thuộc dữ liệu đã seed từ các đợt trước. Chạy lại nhiều lần được, không cần
# dọn DB giữa các lần.
#
# LƯU Ý khi thêm check mới: các handler khai kiểu `Task` (không `IResult`) trả
# HTTP 200, không 204 — đừng mặc định 204 cho endpoint POST/PUT.
# ============================================================================
API=http://localhost:5256/api/v1
TS=$(date +%s)
PASS=0; FAIL=0
ok()   { echo "OK   $1"; PASS=$((PASS+1)); }
bad()  { echo "FAIL $1 — $2"; FAIL=$((FAIL+1)); }
chk()  { if [ "$2" = "$3" ]; then ok "$1"; else bad "$1" "mong đợi $3, nhận $2"; fi }

jqv() { python3 -c "import sys,json;d=json.load(sys.stdin);print(d$1)" 2>/dev/null; }

CAND="smoke-cand-$TS@test.local"
EMP="smoke-emp-$TS@test.local"
PW="Test1234!"

echo "===== A. ỨNG VIÊN: đăng ký → hồ sơ → CCHN → CV ====="
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/auth/register -H 'Content-Type: application/json' -d "{\"email\":\"$CAND\",\"password\":\"$PW\",\"role\":\"candidate\"}")
chk "ứng viên đăng ký" "$C" "200"
CTOK=$(curl -s -X POST $API/auth/login -H 'Content-Type: application/json' -d "{\"email\":\"$CAND\",\"password\":\"$PW\"}" | jqv "['accessToken']")
[ -n "$CTOK" ] && ok "ứng viên đăng nhập" || bad "ứng viên đăng nhập" "không lấy được token"

CA="Authorization: Bearer $CTOK"
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT $API/candidates/me -H "$CA" -H 'Content-Type: application/json' \
  -d '{"fullName":"Đỗ Thị Smoke","headline":"Điều dưỡng ICU","summary":"5 năm hồi sức","address":"Hà Nội","gender":"Female"}')
chk "cập nhật hồ sơ cá nhân" "$C" "200"

CPID=$(curl -s $API/candidates/me -H "$CA" | jqv "['id']")
[ -n "$CPID" ] && ok "đọc lại hồ sơ (có id)" || bad "đọc lại hồ sơ" "thiếu id"

# Học vấn + kinh nghiệm (replace-all)
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT $API/candidates/me/history -H "$CA" -H 'Content-Type: application/json' \
  -d '{"experiences":[{"organizationName":"BV Bạch Mai","position":"Điều dưỡng","tier":"TrungUong","fromDate":"2020-01-01","toDate":null,"description":"ICU"}],"educations":[{"schoolName":"ĐH Y Hà Nội","degree":"Cử nhân","major":"Điều dưỡng","fromYear":2016,"toYear":2020}]}')
chk "lưu học vấn + kinh nghiệm" "$C" "200"
NEXP=$(curl -s $API/candidates/me -H "$CA" | jqv "['experiences'].__len__()")
chk "kinh nghiệm đã lưu" "$NEXP" "1"

# CCHN
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/candidates/me/licenses -H "$CA" -H 'Content-Type: application/json' \
  -d '{"licenseNo":"CCHN-SMOKE-1","issuedBy":"Sở Y tế Hà Nội","issuedAt":"2021-03-01","documentUrl":"https://minio.local/l/1.jpg"}')
chk "thêm CCHN" "$C" "200"

# CV Builder + CV file
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT $API/candidates/me/cvs/builder -H "$CA" -H 'Content-Type: application/json' \
  -d '{"title":"CV Điều dưỡng","dataJson":"{\"education\":[],\"experience\":[],\"skills\":[\"CPR\"]}"}')
chk "lưu CV Builder" "$C" "200"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/candidates/me/cvs/file -H "$CA" -H 'Content-Type: application/json' \
  -d '{"title":"CV tiếng Anh.pdf","fileUrl":"https://minio.local/cv/en.pdf"}')
chk "lưu CV dạng file" "$C" "200"
NCV=$(curl -s $API/candidates/me/cvs -H "$CA" | python3 -c "import sys,json;print(len(json.load(sys.stdin)))")
chk "có 2 CV (builder + file)" "$NCV" "2"

echo
echo "===== B. NTD: đăng ký → tổ chức → giấy tờ → tin tuyển dụng ====="
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/auth/register -H 'Content-Type: application/json' -d "{\"email\":\"$EMP\",\"password\":\"$PW\",\"role\":\"employer\"}")
chk "NTD đăng ký" "$C" "200"
ETOK=$(curl -s -X POST $API/auth/login -H 'Content-Type: application/json' -d "{\"email\":\"$EMP\",\"password\":\"$PW\"}" | jqv "['accessToken']")
EA="Authorization: Bearer $ETOK"
ORGID=$(curl -s -X POST $API/organizations -H "$EA" -H 'Content-Type: application/json' \
  -d "{\"name\":\"BV Smoke $TS\",\"orgType\":\"BenhVienTu\",\"description\":\"Test chốt giai đoạn\",\"address\":\"Hà Nội\"}" | tr -d '"')
[ -n "$ORGID" ] && ok "tạo tổ chức" || bad "tạo tổ chức" "không lấy được id"

C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/organizations/$ORGID/documents -H "$EA" -H 'Content-Type: application/json' \
  -d '{"docType":"operating_license","fileUrl":"https://minio.local/org/gp.pdf"}')
chk "nộp giấy phép hoạt động" "$C" "200"
# docType lạ phải bị chặn
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/organizations/$ORGID/documents -H "$EA" -H 'Content-Type: application/json' \
  -d '{"docType":"buisness_licence","fileUrl":"https://minio.local/org/x.pdf"}')
chk "docType sai chính tả bị chặn" "$C" "400"
# Người ngoài KHÔNG đọc được giấy phép (lỗ hổng đã bịt)
C=$(curl -s -o /dev/null -w '%{http_code}' $API/organizations/$ORGID/documents -H "$CA")
chk "ứng viên KHÔNG đọc được giấy phép tổ chức" "$C" "403"

echo
echo "===== C. VẬN HÀNH: duyệt tổ chức + CCHN → NTD đăng tin ====="
ADMTOK=$(curl -s -X POST $API/auth/login -H 'Content-Type: application/json' -d '{"email":"admin@blousehiding.local","password":"Administrator1!"}' | jqv "['accessToken']")
AA="Authorization: Bearer $ADMTOK"
[ -n "$ADMTOK" ] && ok "Vận hành đăng nhập (tài khoản seed)" || bad "Vận hành đăng nhập" "không lấy token"

C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/verify -H "$AA" -H 'Content-Type: application/json' -d '{"action":"Verify"}')
chk "duyệt xác thực tổ chức" "$C" "200"

LICID=$(curl -s $API/ops/licenses -H "$AA" | python3 -c "
import sys,json
for l in json.load(sys.stdin):
    if l.get('licenseNo')=='CCHN-SMOKE-1': print(l['id']); break")
[ -n "$LICID" ] && ok "CCHN xuất hiện trong hàng đợi duyệt" || bad "hàng đợi CCHN" "không thấy CCHN-SMOKE-1"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/licenses/$LICID/verify -H "$AA" -H 'Content-Type: application/json' -d '{"approved":true}')
chk "duyệt CCHN" "$C" "200"

SPEC=$(curl -s $API/catalog/specialties | python3 -c "import sys,json;print(json.load(sys.stdin)[0]['id'])")
LOC=$(curl -s $API/catalog/locations | python3 -c "import sys,json;print(json.load(sys.stdin)[0]['id'])")
JOBID=$(curl -s -X POST $API/jobs -H "$EA" -H 'Content-Type: application/json' -d "{\"organizationId\":\"$ORGID\",\"title\":\"Điều dưỡng ICU (smoke $TS)\",\"specialtyId\":\"$SPEC\",\"employmentType\":\"FullTime\",\"salaryMin\":null,\"salaryMax\":null,\"salaryNegotiable\":true,\"locationId\":\"$LOC\",\"addressDetail\":null,\"requiredLicense\":true,\"minExperienceYears\":1,\"description\":\"Mô tả công việc smoke test\",\"requirements\":null,\"benefits\":null}" | tr -d '"')
[ -n "$JOBID" ] && ok "NTD tạo tin (draft)" || bad "tạo tin" "không lấy id"

PKG=$(curl -s $API/catalog/job-packages | python3 -c "
import sys,json
for p in json.load(sys.stdin):
    if p.get('price')==0: print(p['id']); break")
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/jobs/$JOBID/submit -H "$EA" -H 'Content-Type: application/json' -d "{\"packageId\":\"$PKG\"}")
chk "nộp duyệt tin (gói Free)" "$C" "200"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/jobs/$JOBID/moderate -H "$AA" -H 'Content-Type: application/json' -d '{"approved":true}')
chk "Vận hành duyệt tin" "$C" "200"

echo
echo "===== D. ỨNG VIÊN: tra cứu → xem tổ chức → nộp hồ sơ ====="
FOUND=$(curl -s "$API/jobs?keyword=smoke%20$TS" | jqv "['totalCount']")
chk "tìm được tin vừa duyệt (tin công khai)" "$FOUND" "1"
ORGPUB=$(curl -s "$API/organizations?q=BV%20Smoke%20$TS" | jqv "['totalCount']")
chk "tổ chức hiện trong danh sách công khai (đã verified)" "$ORGPUB" "1"

CVID=$(curl -s $API/candidates/me/cvs -H "$CA" | python3 -c "
import sys,json
for c in json.load(sys.stdin):
    if c.get('fileUrl'): print(c['id']); break")
APPID=$(curl -s -X POST $API/jobs/$JOBID/applications -H "$CA" -H 'Content-Type: application/json' \
  -d "{\"coverLetter\":\"Tôi quan tâm vị trí này\",\"cvId\":\"$CVID\"}" | tr -d '"')
[ -n "$APPID" ] && ok "nộp hồ sơ bằng CV đã lưu" || bad "nộp hồ sơ" "không lấy id"
# Nộp trùng phải bị chặn
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/jobs/$JOBID/applications -H "$CA" -H 'Content-Type: application/json' -d '{}')
chk "nộp trùng bị chặn" "$C" "400"

echo
echo "===== E. NTD: xem ứng viên đã nộp + tìm ứng viên phù hợp ====="
NAPP=$(curl -s "$API/jobs/$JOBID/applications" -H "$EA" | jqv "['totalCount']")
chk "ATS: thấy 1 đơn ứng tuyển" "$NAPP" "1"
CVURL=$(curl -s $API/applications/$APPID -H "$EA" | jqv "['cvFileUrl']")
[ "$CVURL" != "None" ] && [ -n "$CVURL" ] && ok "NTD thấy link CV của đơn" || bad "NTD thấy link CV" "cvFileUrl=$CVURL"

C=$(curl -s -o /dev/null -w '%{http_code}' -X PATCH $API/applications/$APPID/stage -H "$EA" -H 'Content-Type: application/json' -d '{"stage":"Interview","silent":false,"rejectedReason":null}')
chk "chuyển giai đoạn ATS" "$C" "200"
NHIS=$(curl -s $API/applications/$APPID/history -H "$EA" | python3 -c "import sys,json;print(len(json.load(sys.stdin)))")
[ "$NHIS" -ge 1 ] && ok "lịch sử chuyển giai đoạn được ghi ($NHIS bản ghi)" || bad "lịch sử giai đoạn" "rỗng"

# Ứng viên nhận thông báo đổi giai đoạn
NOTI=$(curl -s "$API/notifications" -H "$CA" | jqv "['totalCount']")
[ "$NOTI" -ge 1 ] && ok "ứng viên nhận thông báo ($NOTI)" || bad "thông báo ứng viên" "rỗng"

echo
echo "===== F. CREDIT: nạp → mở hồ sơ → hoàn khi tranh chấp ====="
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/credit-bonus -H "$AA" -H 'Content-Type: application/json' -d '{"amount":50,"reason":"Smoke test chốt giai đoạn"}')
chk "Vận hành cộng Credit (có lý do)" "$C" "200"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/credit-bonus -H "$AA" -H 'Content-Type: application/json' -d '{"amount":50}')
chk "cộng Credit thiếu lý do bị chặn" "$C" "400"

C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/candidates/$CPID/unlock -H "$EA" -H 'Content-Type: application/json' -d "{\"organizationId\":\"$ORGID\"}")
chk "NTD mở hồ sơ ứng viên (trừ Credit)" "$C" "200"
BAL=$(curl -s $API/organizations/$ORGID/credit-wallet -H "$EA" | jqv "['balance']")
chk "số dư còn 35 (50 - 15)" "$BAL" "35"

# NTD thấy học vấn/kinh nghiệm sau khi mở
NEXP2=$(curl -s "$API/candidates/$CPID?organizationId=$ORGID" -H "$EA" | jqv "['experiences'].__len__()")
chk "NTD thấy kinh nghiệm sau khi mở hồ sơ" "$NEXP2" "1"

UNTX=$(curl -s $API/ops/organizations/$ORGID/refundable-unlocks -H "$AA" | jqv "[0]['transactionId']")
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/credit-refund -H "$AA" -H 'Content-Type: application/json' -d "{\"transactionId\":\"$UNTX\",\"reason\":\"Smoke: hồ sơ trùng\"}")
chk "hoàn Credit tranh chấp" "$C" "200"
BAL2=$(curl -s $API/organizations/$ORGID/credit-wallet -H "$EA" | jqv "['balance']")
chk "số dư về lại 50 sau hoàn" "$BAL2" "50"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/credit-refund -H "$AA" -H 'Content-Type: application/json' -d "{\"transactionId\":\"$UNTX\",\"reason\":\"Hoàn lần 2\"}")
chk "hoàn lần 2 bị chặn" "$C" "400"

echo
echo "===== G. RBAC: chặn vượt quyền ====="
C=$(curl -s -o /dev/null -w '%{http_code}' $API/ops/licenses -H "$CA")
chk "ứng viên KHÔNG vào được hàng đợi Vận hành" "$C" "403"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST $API/ops/organizations/$ORGID/credit-bonus -H "$EA" -H 'Content-Type: application/json' -d '{"amount":999,"reason":"tự cộng"}')
chk "NTD KHÔNG tự cộng Credit được" "$C" "403"
C=$(curl -s -o /dev/null -w '%{http_code}' "$API/jobs/$JOBID/applications")
chk "chưa đăng nhập KHÔNG xem được danh sách ứng viên" "$C" "401"

# rejectReason là ghi chú NỘI BỘ của Vận hành — endpoint /organizations/{id} là Public nên phải
# ẩn field này với khách và người ngoài tổ chức.
ORG2=$(curl -s -X POST $API/organizations -H "$EA" -H 'Content-Type: application/json' \
  -d "{\"name\":\"BV Bị Từ Chối $TS\",\"orgType\":\"PhongKham\"}" 2>/dev/null | tr -d '"')
if [ -z "$ORG2" ]; then
  # NTD chỉ tạo được 1 tổ chức — dùng chính tổ chức đã có, chuyển sang Rejected
  ORG2=$ORGID
fi
curl -s -o /dev/null -X POST $API/ops/organizations/$ORG2/verify -H "$AA" -H 'Content-Type: application/json' \
  -d '{"action":"Reject","rejectReason":"Ghi chú nội bộ: nghi giấy phép làm giả"}'
RR_ANON=$(curl -s "$API/organizations/$ORG2" | jqv "['rejectReason']")
chk "khách KHÔNG thấy rejectReason (ghi chú nội bộ)" "$RR_ANON" "None"
RR_CAND=$(curl -s "$API/organizations/$ORG2" -H "$CA" | jqv "['rejectReason']")
chk "ứng viên KHÔNG thấy rejectReason" "$RR_CAND" "None"
RR_OWNER=$(curl -s "$API/organizations/$ORG2" -H "$EA" | jqv "['rejectReason']")
if [ "$RR_OWNER" != "None" ] && [ -n "$RR_OWNER" ]; then ok "NTD (thành viên) VẪN thấy rejectReason để biết sửa gì"; else bad "NTD thấy rejectReason" "nhận: $RR_OWNER"; fi

echo
echo "======================================"
echo "TỔNG: $PASS đạt / $FAIL không đạt"
[ "$FAIL" -eq 0 ] && echo "==> SMOKE TEST GIAI ĐOẠN 1: ĐẠT" || echo "==> CÒN $FAIL MỤC KHÔNG ĐẠT"
