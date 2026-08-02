import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

export default async function LoginPage() {
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
          <h1 className="text-xl font-semibold text-ink">{t("loginTitle")}</h1>
        </CardHeader>
        <CardContent>
          <form className="space-y-4">
            <div className="space-y-1.5">
              <Label htmlFor="email">{t("email")}</Label>
              <Input id="email" type="email" autoComplete="email" required />
            </div>
            <div className="space-y-1.5">
              <div className="flex items-center justify-between">
                <Label htmlFor="password">{t("password")}</Label>
                <Link
                  href="/auth/forgot-password"
                  className="text-xs text-accent-jade hover:underline"
                >
                  {t("forgotPassword")}
                </Link>
              </div>
              <Input
                id="password"
                type="password"
                autoComplete="current-password"
                required
              />
            </div>
            <Button type="submit" className="w-full">
              {t("submitLogin")}
            </Button>
          </form>

          <div className="my-5 flex items-center gap-3 text-xs text-ink-muted">
            <Separator className="flex-1" />
            {t("orContinueWith")}
            <Separator className="flex-1" />
          </div>

          <div className="grid grid-cols-2 gap-2">
            <Button variant="outline">{t("continueWithGoogle")}</Button>
            <Button variant="outline">{t("continueWithZalo")}</Button>
          </div>

          <p className="mt-6 text-center text-sm text-ink-muted">
            {t("noAccount")}{" "}
            <Link
              href="/auth/register"
              className="font-medium text-accent-jade hover:underline"
            >
              {t("registerTitle")}
            </Link>
          </p>
        </CardContent>
      </Card>
    </main>
  );
}
