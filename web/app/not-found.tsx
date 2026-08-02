// Next.js yêu cầu 1 not-found.tsx ở root khi có segment động [locale] ở app/.
// Trong-locale (vd /vi/khong-ton-tai) không đi qua file này — proxy.ts đã redirect
// mọi request về đúng tiền tố locale trước khi tới đây.
export default function RootNotFound() {
  return (
    <html lang="vi">
      <body>
        <div style={{ padding: 32, fontFamily: "system-ui" }}>
          <h1>404 — Không tìm thấy trang</h1>
        </div>
      </body>
    </html>
  );
}
