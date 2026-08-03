import { backendFetch } from "@/lib/backend-fetch";

export async function PUT(request: Request) {
  const body = await request.json();

  const res = await backendFetch("/candidates/me", {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const data = await res.json().catch(() => null);
  return Response.json(data, { status: res.status });
}
