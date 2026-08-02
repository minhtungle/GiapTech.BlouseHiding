import createMiddleware from "next-intl/middleware";
import { routing } from "./i18n/routing";

// Next.js 16 đổi tên file convention "middleware.ts" -> "proxy.ts" (hàm bên trong
// không đổi hợp đồng request/response, chỉ đổi tên file + export). Xem
// node_modules/next/dist/docs/01-app/03-api-reference/03-file-conventions/proxy.md
export default createMiddleware(routing);

export const config = {
  // Bỏ qua API, file tĩnh Next.js, và file có phần mở rộng (favicon, ảnh...)
  matcher: ["/((?!api|_next|.*\\..*).*)"],
};
