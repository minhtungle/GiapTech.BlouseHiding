import { getAccessToken, getRefreshToken, setAuthCookies, clearAuthCookies } from "@/lib/auth-cookies";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100/api/v1";

// Gọi backend kèm access token từ cookie httpOnly; tự refresh 1 lần nếu access token hết hạn (401).
// Dùng trong Route Handler (app/api/*) — Client Component không đọc được cookie httpOnly nên phải
// proxy qua đây thay vì gọi thẳng backend từ trình duyệt.
export async function backendFetch(path: string, init: RequestInit = {}): Promise<Response> {
  const accessToken = await getAccessToken();

  const doFetch = (token: string | undefined) =>
    fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        ...init.headers,
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

  let res = await doFetch(accessToken);

  if (res.status === 401) {
    const refreshToken = await getRefreshToken();
    if (refreshToken) {
      const refreshRes = await fetch(`${API_BASE_URL}/auth/refresh`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken }),
      });

      if (refreshRes.ok) {
        const data = await refreshRes.json();
        await setAuthCookies(data.accessToken, data.refreshToken);
        res = await doFetch(data.accessToken);
      } else {
        await clearAuthCookies();
      }
    }
  }

  return res;
}
