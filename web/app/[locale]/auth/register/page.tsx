import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { RegisterForm } from "./register-form";

export default async function RegisterPage() {
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
            {t("registerTitle")}
          </h1>
        </CardHeader>
        <CardContent>
          <RegisterForm />

          <p className="mt-6 text-center text-sm text-ink-muted">
            {t("haveAccount")}{" "}
            <Link
              href="/auth/login"
              className="font-medium text-accent-jade hover:underline"
            >
              {t("loginTitle")}
            </Link>
          </p>
        </CardContent>
      </Card>
    </main>
  );
}
