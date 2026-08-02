<title>GiapTech.BlouseHiding — Tổng thuật hệ thống</title>

# GiapTech.BlouseHiding — Tổng thuật hệ thống

*Viết cho người mới tiếp cận — đọc một mạch để hiểu toàn cảnh, sau đó tra cứu chi tiết ở bản đồ tài liệu cuối bài.*

---

## 1. Hệ thống này giải quyết vấn đề gì

GiapTech.BlouseHiding là một nền tảng tuyển dụng, nhưng **chỉ dành cho ngành y tế** — bác sĩ, điều dưỡng, dược sĩ, kỹ thuật viên xét nghiệm, hộ lý, nữ hộ sinh tìm việc tại bệnh viện, phòng khám, nhà thuốc, công ty dược. Về hình thức nó giống TopCV (đăng tin — tìm việc — ứng tuyển) và có vay mượn một phần tinh thần cộng đồng của Ybox (sự kiện, nội dung nghề nghiệp), nhưng có một khác biệt cố ý và xuyên suốt: **xác thực chứng chỉ hành nghề (CCHN)**.

Lý do khác biệt này tồn tại: các nền tảng tuyển dụng đại trà không có cơ chế nào chứng minh một "bác sĩ" đăng hồ sơ thật sự có quyền hành nghề. Với ngành y — nơi sai người có thể ảnh hưởng tính mạng bệnh nhân — đây không phải tính năng phụ, mà là **lý do nền tảng này tồn tại**. Toàn bộ thiết kế, từ mô hình dữ liệu đến giao diện, xoay quanh việc làm cho "đã xác thực CCHN" trở thành tín hiệu tin cậy rõ ràng, dễ thấy, khó giả mạo.

Ba đặc thù khác của ngành y được đưa thẳng vào thiết kế thay vì để sau: **chuyên khoa** (một danh mục chuẩn hóa — Nội, Ngoại, Sản, Nhi, Gây mê hồi sức...) để tìm kiếm/khớp lệnh chính xác; **loại hình làm việc đặc thù** (trực ca, locum — làm thay theo buổi, cộng tác viên khám bệnh) mà tuyển dụng đại trà không có khái niệm; và **dữ liệu cá nhân cực kỳ nhạy cảm** (CCHN, giấy phép hành nghề) buộc phải tuân thủ Nghị định 13/2023/NĐ-CP ngay từ tầng thiết kế dữ liệu, không phải bổ sung sau.

---

## 2. Ai dùng hệ thống, dùng ở đâu

Hệ thống có ba nhóm người dùng, và — điểm quan trọng cần nắm ngay để không nhầm lẫn khi đọc phần sau — **ba khu vực giao diện mang ba cái tên khác nhau, đừng suy diễn theo trực giác thông thường**:

| Khu vực (site) | Dành cho | Làm gì ở đó |
|---|---|---|
| **Client** | Ứng viên & khách chưa đăng nhập | Tìm việc, xem tin, tạo hồ sơ, ứng tuyển, chat |
| **Admin** | **Nhà tuyển dụng (NTD)** — cơ sở y tế/HR | Đăng tin, quản lý CV/ATS, mua gói, quản lý Credit |
| **Vận hành** | Đội nội bộ nền tảng (không phải NTD) | Duyệt CCHN, duyệt doanh nghiệp, duyệt tin, xử lý report |

Cái tên "Admin" ở đây **không phải** trang quản trị nội bộ như trực giác thường nghĩ — nó là trang dành cho khách hàng trả tiền (Nhà tuyển dụng). Đội ngũ nội bộ vận hành nền tảng dùng một khu vực tên khác hẳn: **Vận hành**. Sự tách bạch này từng bị đặt tên trùng nhau trong một phiên bản tài liệu trước, gây nhầm lẫn thật sự, nên đã được ghi lại thành quyết định kiến trúc chính thức (ADR-0005) để không lặp lại.

Về mặt kỹ thuật, ba khu vực này thực chất chia thành **hai ứng dụng frontend**: **Client** là một app Next.js riêng (cần SEO/SSR cho tin tuyển dụng lên Google), còn **Admin** và **Vận hành** dùng chung **một** ứng dụng khác — dựng thẳng trên **shadcn-admin** (một admin dashboard mã nguồn mở có sẵn), phân biệt màn hình theo vai trò đăng nhập chứ không tách thêm ứng dụng thứ ba. Quyết định ban đầu là ép cả ba vào một codebase Next.js duy nhất để dễ kiểm soát; sau khi dựng thử mockup thực tế mới nhận ra Admin/Vận hành không cần bất kỳ lợi ích nào của Next.js (không SEO, không chia sẻ URL công khai với Client) trong khi phải tự dựng lại toàn bộ pattern mà shadcn-admin đã có sẵn — nên quyết định đảo ngược, dùng thẳng shadcn-admin thay vì chỉ "tham khảo bố cục" (ghi lại ở ADR-0008).

---

## 3. Một tin tuyển dụng, từ lúc sinh ra đến lúc có người trúng tuyển

Cách tốt nhất để hiểu một hệ thống là đi theo một đối tượng dữ liệu xuyên suốt vòng đời của nó. Hãy theo một tin tuyển dụng cụ thể: **"Điều dưỡng ICU — Ca đêm"** của Bệnh viện Đa khoa Tâm Anh.

**Bước 0 — trước khi có tin, tổ chức phải tồn tại và được xác minh.** Đại diện bệnh viện đăng ký tài khoản NTD, tạo hồ sơ tổ chức, upload giấy phép hoạt động khám chữa bệnh. Tổ chức ở trạng thái "chờ xác minh" cho tới khi đội Vận hành đối chiếu giấy phép và duyệt. Không có bước này, **không tin nào của tổ chức được phép công khai** — đây là ràng buộc cứng, enforce ở tầng ứng dụng chứ không chỉ tin vào UI: `jobs.status` chỉ chuyển sang `published` khi `organizations.verify_status = verified`.

**Bước 1 — soạn tin, ở trạng thái nháp.** HR điền vị trí, chuyên khoa (Hồi sức cấp cứu), loại hình (Trực ca), mức lương, yêu cầu CCHN, kinh nghiệm tối thiểu. Tin nằm ở `draft`, sửa thoải mái.

**Bước 2 — chọn gói và nộp duyệt.** Đến đây có một nhánh rẽ quan trọng phản ánh đúng thực tế vận hành hiện tại của dự án: hệ thống **chưa có cổng thanh toán tự động** (VNPay/Momo/ZaloPay — quyết định hoãn lại có chủ đích, ghi trong ADR-0003). Nếu HR chọn gói miễn phí, tin đi thẳng vào hàng đợi duyệt nội dung. Nếu chọn gói trả phí (Eco/Pro/Max — khác nhau về thời gian hiển thị và vị trí ưu tiên khi tìm kiếm), tin chuyển sang một trạng thái riêng: **`pending_payment`** — chờ thanh toán, **khác hẳn** với `pending` — chờ duyệt nội dung. Hai ý nghĩa này ban đầu bị gộp làm một trong bản thiết kế đầu tiên, và đã được tách ra sau một vòng rà soát vì gộp lại sẽ khiến đội Vận hành nhìn nhầm "tin đang chờ tiền" thành "tin sẵn sàng duyệt nội dung".

**Bước 3 — chuyển khoản và đối soát thủ công.** Hệ thống hiển thị số tài khoản và một **mã tham chiếu** duy nhất (vd `PAY-7F3K2Q`) mà HR phải ghi đúng vào nội dung chuyển khoản. Đội Vận hành có một màn hình riêng để xem các giao dịch đang chờ, đối chiếu với sao kê ngân hàng theo mã này, rồi bấm xác nhận — hành động này trong cùng một transaction vừa đánh dấu giao dịch thành công, vừa tạo bản ghi mua gói, vừa đẩy tin từ `pending_payment` sang `pending`. Nếu số tiền sai hoặc không tìm thấy giao dịch, Vận hành từ chối, tin quay về `draft` để HR sửa và nộp lại. Đây là quy trình có thật, không phải giả định — chi tiết công cụ (mã lưu ở đâu, ai xác nhận, index nào cần có để tra cứu nhanh) đã có trong ERD, không chỉ nằm trong mô tả nghiệp vụ.

**Bước 4 — duyệt nội dung.** Đội Vận hành (vai trò Moderator) kiểm tra tin có minh bạch mức lương không, có ngôn từ phân biệt đối xử không, có đúng quy định quảng cáo tuyển dụng ngành y không. Duyệt → `published`, hiển thị công khai và lên kết quả tìm kiếm. Từ chối → quay lại `draft` kèm lý do cụ thể — nguyên tắc xuyên suốt toàn hệ thống là **không bao giờ từ chối chung chung**, luôn nói rõ vì sao.

**Bước 5 — ứng viên nộp đơn.** Một ứng viên tìm thấy tin, xem chi tiết (thấy rõ badge "Yêu cầu CCHN Điều dưỡng"), chọn CV, ứng tuyển. Ngay tại thời điểm này, hệ thống **chụp lại một bản sao (snapshot) của CV** và tính điểm chấm hồ sơ ban đầu dựa trên đúng bản đó. Vì sao cần bước này: nếu ứng viên sau đó sửa CV, NTD xem lại đơn ứng tuyển cũ vẫn phải thấy đúng những gì đã được nộp lúc đó — không phải bản đã bị chỉnh sửa ngược. Đây cũng là lỗ hổng thực sự được phát hiện khi rà soát lại thiết kế: bản phác thảo đầu tiên dùng tham chiếu sống tới CV, sau đó được sửa thành bản chụp cố định.

**Bước 6 — HR xử lý qua bảng Kanban.** Ứng viên đi qua 6 cột: Mới → Đang xem → Phù hợp → Hẹn phỏng vấn → Offer → Trúng tuyển, với nhánh rẽ "Từ chối" ở bất kỳ bước nào. Mỗi lần chuyển cột đều ghi lịch sử và gửi thông báo cho ứng viên (trừ khi HR chọn "âm thầm"). Bảng này **không đóng lại** dù tin đã hết hạn hay bị đóng sớm — chỉ phần hiển thị công khai của tin bị ảnh hưởng, ứng viên đang xử lý dở dang không bị bỏ rơi giữa chừng.

**Và nếu có chuyện xấu xảy ra?** Hai tình huống được xử lý rõ ràng thay vì bỏ ngỏ: nếu tổ chức bị phát hiện gian lận sau khi đã xác thực (qua báo cáo vi phạm), việc rút xác thực **tự động** ẩn toàn bộ tin đang công khai của tổ chức đó trong cùng một thao tác — không phải việc Vận hành phải nhớ đi ẩn từng tin một; muốn hiện lại phải xác thực lại tổ chức **và** duyệt lại từng tin thủ công, không tự động bật lại. Còn khi tin hết hạn và HR muốn "gia hạn", hệ thống **tạo một tin hoàn toàn mới** sao chép nội dung, tin cũ tự đóng — nhờ vậy một ứng viên từng bị từ chối ở đợt tuyển trước vẫn có thể ứng tuyển lại ở đợt mới, vì đó kỹ thuật là một tin khác.

---

## 4. Một ứng viên, từ lúc đăng ký đến lúc được tin cậy

Ứng viên đăng ký bằng email/SĐT (có OTP) hoặc OAuth Google/Zalo, tạo hồ sơ cơ bản (học vấn, kinh nghiệm), rồi tới phần quan trọng nhất: nhập số CCHN, nơi cấp, ngày hết hạn, upload ảnh chứng chỉ. Hồ sơ ở trạng thái "chưa xác thực" cho tới khi đội Vận hành đối chiếu ảnh và duyệt.

Một quyết định thiết kế đáng chú ý ở đây: hồ sơ **chưa xác thực CCHN vẫn dùng để ứng tuyển được**, chỉ gắn nhãn rõ ràng cho NTD biết. Đây là sự đánh đổi có chủ đích — nếu bắt buộc xác thực xong mới cho thao tác gì, tỷ lệ người dùng mới bỏ cuộc ngay từ đăng ký sẽ rất cao (tham khảo chiến lược "đăng ký nhanh trước, xác thực sau" phổ biến ở TopCV). Cái giá phải trả cho sự linh hoạt này là NTD phải luôn được thấy trạng thái thật, không được che giấu.

CCHN có ngày hết hạn — khi quá hạn, một job nền tự động chuyển trạng thái sang "hết hiệu lực" và nhắc ứng viên gia hạn, không tự ý xóa liên kết dữ liệu cũ.

Sau khi có hồ sơ, ứng viên tìm việc, lọc theo chuyên khoa/địa điểm/loại hình, ứng tuyển, rồi theo dõi trạng thái đơn qua thông báo. Ngoài con đường chủ động này, còn một con đường thứ hai đặc trưng của ngành y: **NTD chủ động tìm và liên hệ ứng viên** — vì nhân sự y tế giỏi thường không tự đi nộp đơn ở nhiều nơi. Đây là nơi cơ chế Credit xuất hiện.

---

## 5. Mô hình kiếm tiền: gói tin và Credit, tại sao có cả hai

Nền tảng có hai nguồn thu song song, học trực tiếp từ TopCV nhưng áp dụng có chọn lọc:

**Gói đăng tin (Eco/Pro/Max)** — NTD trả tiền để tin hiển thị lâu hơn và ưu tiên hơn trong kết quả tìm kiếm. Đây là mô hình thu tiền quen thuộc của mọi job board.

**Credit để "mở" hồ sơ ứng viên** — NTD trả điểm để xem đầy đủ thông tin liên hệ của một ứng viên tìm thấy qua tìm kiếm chủ động, thay vì chỉ chờ họ tự ứng tuyển. Lý do mô hình này quan trọng riêng cho ngành y: nhân sự y tế có chuyên môn hiếm (như Gây mê hồi sức) thường không chủ động rải đơn, nên nếu chỉ có mô hình đăng tin — chờ ứng tuyển thuần túy, NTD sẽ rất khó tuyển được người giỏi. Việc mở hồ sơ là **theo tổ chức, không theo từng nhân viên HR** — một khi tổ chức đã mở, mọi HR trong tổ chức đó xem được mãi mãi mà không mất thêm Credit, và xem lại lịch sử không tính phí thêm lần hai.

Cả hai luồng tiền hiện tại đều đi qua **một quy trình thủ công chung**: chuyển khoản kèm mã tham chiếu, đội Vận hành đối soát và xác nhận. Đây không phải một khiếm khuyết bị bỏ sót — nó là một quyết định kiến trúc ghi rõ trong ADR-0003: hoãn chọn cổng thanh toán tự động để không chặn tiến độ, nhưng thiết kế schema/API **sẵn sàng cắm cổng tự động vào sau** mà không phải đổi mô hình dữ liệu — chỉ cần thêm một giá trị `provider` mới bên cạnh `manual_transfer`.

Có một cơ chế hoàn tiền (Credit refund) cho các trường hợp tranh chấp — mở nhầm hồ sơ, lỗi hệ thống trừ sai — nhưng đây là hành động **thủ công có chủ đích** của Vận hành, không có luồng hoàn tự động, vì hoàn tiền tự động cho một hành động vốn không thể "hoàn tác" thật sự (thông tin liên hệ đã bị xem) là một quyết định cần con người cân nhắc từng trường hợp.

---

## 6. Kiến trúc kỹ thuật: chọn sự đơn giản có chủ đích

Ba nguyên tắc kiến trúc chi phối mọi quyết định công nghệ phía sau:

**Modular Monolith, không microservice ngay.** Hệ thống có nhiều miền nghiệp vụ rõ ràng (Identity, Profile, Job, Application, Messaging, Ops, Events), nhưng chúng sống chung trong một ứng dụng backend, giao tiếp nội bộ qua MediatR thay vì network call giữa các service riêng. Microservice giải quyết đúng vấn đề "scale độc lập từng miền" — một vấn đề dự án này chưa có, vì đội ngũ còn nhỏ và tải chưa xác định. Ranh giới module vẫn được giữ rõ ràng (mỗi miền là một thư mục riêng trong tầng Application), nên nếu tương lai thật sự cần tách, việc tách sẽ không phải viết lại từ đầu.

**Clean Architecture** — tách rõ bốn tầng Domain / Application / Infrastructure / Web, với quy tắc phụ thuộc chỉ đi một chiều: Domain không được biết gì về các tầng phía trên. Đây là "luật chơi bắt buộc" ghi thẳng trong tài liệu hướng dẫn cho AI agent (CLAUDE.md) — nếu một thay đổi cần import ngược chiều, đó là dấu hiệu thiết kế sai chỗ, không phải quy tắc cần phá.

**Self-host VPS, không dùng cloud managed.** Toàn bộ hạ tầng (database, cache, message queue, object storage, log, giám sát) tự vận hành trên VPS bằng Docker Compose, thay vì Azure/AWS managed service. Hệ quả trực tiếp: những gì cloud managed vốn lo sẵn (backup, patching, high availability) giờ đội ngũ phải tự chịu trách nhiệm — nên có hẳn một quyển "runbook vận hành" quy định tần suất backup, quy trình diễn tập khôi phục, quy trình xử lý sự cố, không để những việc này chỉ tồn tại trong đầu một người.

Về mặt công nghệ cụ thể: backend là **ASP.NET Core .NET 10**, khởi tạo từ khung Clean Architecture có sẵn của cộng đồng (Jason Taylor's Template) thay vì tự dựng lại — một ví dụ của nguyên tắc lớn hơn xuyên suốt dự án: **ưu tiên mã nguồn mở đã kiểm chứng ở tầng nền tảng, tự viết ở tầng tạo khác biệt**. Frontend là **Next.js + shadcn/ui** — shadcn/ui bản chất là mã nguồn mở dạng copy-code vào repo (không phải gói cài đóng kín), cho phép giữ 100% quyền tùy biến giao diện riêng trong khi vẫn thừa hưởng nền tảng accessibility đã được kiểm chứng.

Hệ thống hỗ trợ **6 ngôn ngữ — Tiếng Việt (mặc định), Anh, Nhật, Trung, Hàn, Tây Ban Nha** — phục vụ nhóm ứng viên/tổ chức nước ngoài (bệnh viện có vốn đầu tư nước ngoài, NGO y tế, chuyên gia expat, đặc biệt từ Nhật/Hàn/Trung — các quốc gia đầu tư y tế/dược lớn tại Việt Nam) tồn tại thật trong thị trường tuyển dụng y tế Việt Nam, dù không phải nhóm người dùng chủ đạo. Ngôn ngữ được nhận diện qua **tiền tố URL** (`/vi/...`, `/en/...`...), tự động gợi ý theo trình duyệt khi truy cập lần đầu, và người dùng có thể tự đổi bất kỳ lúc nào qua một nút chọn ngôn ngữ đặt ở góc trên giao diện — bố trí quen thuộc trên nhiều trang web lớn. Phạm vi dịch được **giới hạn có chủ đích**: chỉ dịch giao diện (nút, nhãn, thông báo, email) và danh mục chuẩn hóa do nền tảng quản lý (chuyên khoa, địa điểm, loại hình làm việc) — với 6 ngôn ngữ, bản dịch danh mục được lưu trong một bảng riêng thay vì thêm cột song song cho từng ngôn ngữ, để việc thêm ngôn ngữ thứ 7 sau này không phải sửa cấu trúc bảng. Nội dung do NTD/ứng viên **tự viết** — mô tả tin tuyển dụng, tiểu sử — **không bị dịch**, hiển thị nguyên văn ngôn ngữ tác giả đã nhập dù người xem đang dùng giao diện ngôn ngữ nào. Quyết định không dịch máy tự động xuất phát từ một rủi ro cụ thể: dịch sai thuật ngữ y khoa (tên chuyên khoa, yêu cầu chứng chỉ) có thể gây hiểu nhầm nghiêm trọng hơn là hữu ích.

---

## 7. Mô hình dữ liệu, kể bằng lời thay vì liệt kê cột

Nhìn tổng thể, dữ liệu chia thành sáu cụm liên kết với nhau:

Một `user` có thể là ứng viên, thành viên của một tổ chức tuyển dụng, hoặc nhân viên vận hành — vai trò quyết định site nào họ dùng được. Một ứng viên có đúng một `candidate_profile`, và hồ sơ đó có nhiều `license` (CCHN) — mỗi CCHN xác thực độc lập, không phải một cờ chung "đã xác thực" cho cả hồ sơ, vì một người có thể có nhiều chứng chỉ với phạm vi khác nhau.

Một tổ chức (`organization`) có nhiều thành viên HR (`employer_member`, với vai trò owner/quản lý/thành viên khác nhau), và có thể mời người **chưa từng có tài khoản** tham gia — lời mời được lưu riêng (`organization_invitation`) chờ người đó đăng ký xong mới thực sự trở thành thành viên, tránh tình huống hệ thống phải tạo một "thành viên" không gắn với ai cả.

Một tin tuyển dụng (`job`) thuộc về một tổ chức, gắn với một gói (`job_package`) qua bản ghi mua gói (`job_purchase`) — bản ghi này trỏ tới một giao dịch thanh toán (`payment`) mang mã tham chiếu dùng để đối soát thủ công.

Ứng tuyển (`application`) nối một ứng viên với một tin, mang theo bản chụp CV cố định tại thời điểm nộp, điểm chấm hồ sơ, và trạng thái pipeline — mỗi lần đổi trạng thái đều có một dòng lịch sử riêng (`application_stage_history`), không cho phép "nhảy cóc" trạng thái mà không để lại dấu vết.

Credit sống trong một ví riêng theo tổ chức (`credit_wallet`), mọi biến động (nạp, trừ khi mở hồ sơ, hoàn tiền) đều có một dòng giao dịch (`credit_transaction`) ghi rõ lý do — không có phép cộng/trừ số dư trực tiếp mà không qua bản ghi giao dịch.

Và bao trùm tất cả là một cuốn nhật ký kiểm toán (`audit_log`) bất biến — không sửa, không xóa — ghi lại mọi thao tác nhạy cảm (duyệt CCHN, xóa tài khoản, thay đổi giao dịch), phục vụ tra cứu khi có tranh chấp và là bằng chứng tuân thủ Nghị định 13/2023.

---

## 8. Những ràng buộc đã được siết chặt sau một vòng rà soát nghiêm túc

Bản thiết kế đầu tiên trông hợp lý trên bề mặt, nhưng một vòng rà soát có chủ đích (đọc lại với tinh thần phản biện, không tự xác nhận lại những gì đã viết) đã phát hiện năm lỗ hổng thật sự — những thứ sẽ thành bug hoặc bế tắc nếu bắt tay vào code mà không sửa trước:

Tài liệu API từng mô tả luồng thanh toán tự động qua webhook, mâu thuẫn thẳng với quyết định đã hoãn cổng thanh toán — nếu không phát hiện, người viết code sẽ dựng nhầm cả một luồng không dùng tới. Đội Vận hành được mô tả là "đối soát thanh toán thủ công" trong hướng dẫn sử dụng, nhưng ban đầu không có endpoint API nào tương ứng cho hành động đó. Bảng giao dịch thanh toán không có chỗ lưu mã tham chiếu — một chi tiết nhỏ nhưng nếu thiếu thì cả quy trình đối soát thủ công không thể vận hành được. Trạng thái tin tuyển dụng gộp chung "chờ thanh toán" và "chờ duyệt nội dung" làm một, gây nhầm lẫn hàng đợi cho Vận hành. Và CV được tham chiếu sống thay vì chụp lại tại thời điểm ứng tuyển, khiến việc đánh giá ứng viên trở nên không công bằng nếu họ sửa hồ sơ sau khi đã nộp đơn.

Toàn bộ năm điểm này đã được sửa trực tiếp vào ERD và tài liệu API, không chỉ ghi nhận là "biết rồi để đó". Đây cũng là lý do tài liệu thiết kế của dự án này được viết ở dạng có thể rà soát lại được — mỗi ràng buộc nghiệp vụ quan trọng đều liệt kê tường minh thành một mục riêng trong ERD, không rải rác ẩn trong mô tả luồng.

---

## 9. Thiết kế giao diện: "Tin cậy lâm sàng"

Phong cách hình ảnh được chọn có chủ đích để tránh hai khuôn mẫu đã có sẵn trên thị trường: xanh dương công sở kiểu TopCV, và sặc sỡ trẻ trung kiểu Ybox. Bảng màu dùng một **màu đỏ triện** (gợi con dấu đỏ trên giấy tờ hành chính/y tế Việt Nam) cho các trạng thái xác thực và hành động chính, và một **màu xanh ngọc trầm** cho các yếu tố chuyên môn — hai màu này có ý nghĩa nghiệp vụ thật, không phải chọn ngẫu nhiên. Nguyên tắc ưu tiên hàng đầu là **rõ ràng hơn đẹp thuần túy**, vì người dùng gồm nhiều bác sĩ/điều dưỡng ở nhiều độ tuổi, không phải dân công nghệ trẻ quen giao diện phức tạp.

Về việc dùng mã nguồn mở: khu vực **Client** (đối ngoại, bề mặt tiếp thị/khám phá việc làm) **không** lấy nguyên một theme có sẵn — chỉ dùng shadcn/ui làm nền tảng component rồi tùy biến hoàn toàn theo token màu/chữ riêng, giữ trọn bản sắc "Tin cậy lâm sàng". Khu vực **Admin (NTD) và Vận hành** — sau khi dựng mockup thử mới nhận ra bản sắc branded đầy đủ không hợp với một công cụ mở ra hàng chục lần/ngày để xử lý công việc — chuyển sang giao diện dashboard **chuẩn shadcn/ui trung tính** (ADR-0007), và chạy thẳng trên **shadcn-admin** như một ứng dụng thật (ADR-0008) thay vì chỉ tham khảo bố cục rồi tự dựng lại: không dùng serif tiêu đề, không mảng màu trang trí rộng, chỉ giữ màu thương hiệu cho badge trạng thái và một nút CTA chính mỗi màn hình.

---

## 10. Lộ trình phát triển

Giai đoạn 0 khởi tạo solution và hạ tầng dev. Giai đoạn 1 (MVP) đưa vào **đầy đủ** các hạng mục cốt lõi ngay từ đầu — bao gồm cả mô hình kiếm tiền (gói tin + Credit) thay vì để dành cho giai đoạn sau, vì đây là nguồn doanh thu chính không nên trì hoãn. Giai đoạn 2 bổ sung tìm kiếm nâng cao (OpenSearch), gợi ý việc làm, chat realtime, ứng dụng di động. Giai đoạn 3 mở rộng sang lớp cộng đồng/nội dung kiểu Ybox — sự kiện, CME, bài viết nghề nghiệp — và tiến tới tích hợp xác thực CCHN với dữ liệu ngành khi điều kiện cho phép.

---

## 11. Những gì đã chốt, và những gì còn để ngỏ

**Đã chốt, không cần bàn lại:** stack backend .NET 10/ASP.NET Core; Web trước, chưa làm mobile; hạ tầng self-host VPS; kiến trúc Clean Architecture + Modular Monolith; frontend chia hai app — Client (Next.js + shadcn/ui, bản sắc riêng, 6 ngôn ngữ) và Admin+Vận hành (một app shadcn-admin dùng chung, giao diện trung tính, chỉ tiếng Việt); đa ngôn ngữ 6 thứ tiếng vi/en/ja/zh/ko/es cho Client, routing theo tiền tố URL (chỉ giao diện + danh mục, không dịch nội dung tự viết); cổng thanh toán tự động hoãn lại, dùng quy trình thủ công có đầy đủ hỗ trợ kỹ thuật (mã tham chiếu, endpoint xác nhận); tổ chức bị rút xác thực thì tự động ẩn tin; gia hạn tin luôn tạo tin mới.

**Còn để ngỏ, cần cân nhắc thêm trước hoặc trong lúc code:** chọn nhà cung cấp VPS cụ thể (trong nước hay quốc tế — có phân tích đánh đổi sẵn, nghiêng nhẹ về trong nước vì lý do tuân thủ dữ liệu); phạm vi chính xác của thuật toán chấm điểm CV (quy tắc tự động cụ thể ra sao, hay ban đầu chỉ gắn nhãn thủ công); và cổng thanh toán tự động cụ thể sẽ chọn khi tới lúc cần (PayOS hay tự nối từng cổng riêng) — không có mục nào trong số này chặn việc bắt đầu code.

Một câu hỏi đáng đặt ra khi đánh giá tài liệu này: **liệu quy trình thủ công (đối soát thanh toán, duyệt CCHN, duyệt tin) có tạo ra nút thắt cổ chai khi số lượng người dùng tăng lên không?** Đây là đánh đổi có ý thức để đơn giản hóa MVP, nhưng là điểm nên theo dõi sát khi có số liệu thực tế — nếu đội Vận hành trở thành điểm nghẽn, đó là tín hiệu cần đẩy nhanh việc tự động hóa (cổng thanh toán, và có thể một phần duyệt CCHN) sớm hơn dự kiến.

---

## 12. Bản đồ tài liệu — đọc sâu hơn phần nào

| Muốn hiểu sâu về... | Đọc file |
|---|---|
| Toàn bộ kiến trúc & quyết định công nghệ, trạng thái quyết định | `docs/kien-truc/TONG-QUAN-KIEN-TRUC.md` |
| Vì sao từng quyết định lớn được chọn | `docs/kien-truc/adr/` (6 ADR) |
| Thuật ngữ nghiệp vụ & quy ước tên site | `docs/kien-truc/THUAT-NGU.md` |
| Phân tích nghiệp vụ, đối chiếu TopCV/Ybox, roadmap, rủi ro | `docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md` |
| Luồng nghiệp vụ chi tiết theo sơ đồ + danh sách màn hình | `docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md` |
| Checklist tính năng theo giai đoạn | `docs/nghiep-vu/DANH-SACH-TINH-NANG.md` |
| Quy ước code, layer, CQRS backend | `docs/backend/KIEN-TRUC-BACKEND.md` |
| Thư viện backend cụ thể | `docs/backend/CONG-NGHE-BACKEND.md` |
| Toàn bộ endpoint API | `docs/backend/API-DESIGN.md` |
| Schema CSDL đầy đủ + ràng buộc nghiệp vụ | `docs/database/ERD-CHI-TIET.md` |
| Thư viện frontend, phong cách thiết kế, wireframe | `docs/frontend/` |
| Hạ tầng self-host VPS, runbook vận hành | `docs/ha-tang/` |
| Hướng dẫn sử dụng theo vai trò | `docs/huong-dan-su-dung/` |
| Quy tắc bắt buộc dành cho AI agent khi code | `CLAUDE.md` (gốc repo) |
