import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { ResetPasswordForm } from "./reset-password-form";

export default async function ResetPasswordPage({
  searchParams,
}: {
  searchParams: Promise<{ email?: string }>;
}) {
  const t = await getTranslations("auth");
  const tCommon = await getTranslations("common");
  const { email } = await searchParams;

  return (
    <main className="flex flex-1 items-center justify-center bg-paper px-4 py-16">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <Link
            href="/"
            className="mb-2 font-heading text-lg font-semibold text-ink"
          >
            {tCommon("brand")}
          </Link>
          <h1 className="text-xl font-semibold text-ink">
            {t("resetPasswordTitle")}
          </h1>
          <p className="text-sm text-ink-muted">
            {t("resetPasswordDescription")}
          </p>
        </CardHeader>
        <CardContent>
          <ResetPasswordForm email={email ?? ""} />
        </CardContent>
      </Card>
    </main>
  );
}
