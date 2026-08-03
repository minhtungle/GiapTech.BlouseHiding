import { backendFetch } from "@/lib/backend-fetch";
import { clearAuthCookies } from "@/lib/auth-cookies";

export async function DELETE() {
  const res = await backendFetch("/users/me", { method: "DELETE" });

  if (res.ok) {
    await clearAuthCookies();
  }

  return new Response(null, { status: res.status });
}
