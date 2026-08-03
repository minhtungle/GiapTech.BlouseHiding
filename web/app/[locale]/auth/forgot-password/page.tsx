import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { ForgotPasswordForm } from "./forgot-password-form";

export default async function ForgotPasswordPage() {
  const t = await getTranslations("auth");
  const tCommon = await getTranslations("common");

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
            {t("forgotPasswordTitle")}
          </h1>
          <p className="text-sm text-ink-muted">
            {t("forgotPasswordDescription")}
          </p>
        </CardHeader>
        <CardContent>
          <ForgotPasswordForm />

          <p className="mt-6 text-center text-sm text-ink-muted">
            <Link
              href="/auth/login"
              className="font-medium text-accent-jade hover:underline"
            >
              {t("backToLogin")}
            </Link>
          </p>
        </CardContent>
      </Card>
    </main>
  );
}
