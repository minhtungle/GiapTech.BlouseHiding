import { getAccessToken } from "@/lib/auth-cookies";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100/api/v1";

export type CurrentUser = {
  id: string;
  email: string;
  role: string;
  status: string;
  emailVerified: boolean;
};

// Đọc user hiện tại trong Server Component qua cookie httpOnly — trả null nếu chưa đăng nhập/token
// hết hạn (chưa refresh tự động ở đây, refresh xảy ra khi client gọi API cần auth và nhận 401).
export async function getCurrentUser(): Promise<CurrentUser | null> {
  const accessToken = await getAccessToken();
  if (!accessToken) return null;

  try {
    const res = await fetch(`${API_BASE_URL}/users/me`, {
      headers: { Authorization: `Bearer ${accessToken}` },
      cache: "no-store",
    });
    if (!res.ok) return null;
    return (await res.json()) as CurrentUser;
  } catch {
    return null;
  }
}
