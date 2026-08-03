import { backendFetch } from "@/lib/backend-fetch";

export async function PUT(
  request: Request,
  { params }: { params: Promise<{ licenseId: string }> },
) {
  const { licenseId } = await params;
  const body = await request.json();

  const res = await backendFetch(`/candidates/me/licenses/${licenseId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const data = await res.json().catch(() => null);
  return Response.json(data, { status: res.status });
}

export async function DELETE(
  _request: Request,
  { params }: { params: Promise<{ licenseId: string }> },
) {
  const { licenseId } = await params;

  const res = await backendFetch(`/candidates/me/licenses/${licenseId}`, {
    method: "DELETE",
  });

  return new Response(null, { status: res.status });
}
