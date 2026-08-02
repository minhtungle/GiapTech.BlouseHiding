import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";

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
          <form className="space-y-4">
            <div className="space-y-2">
              <Label>{t("registerTitle")}</Label>
              <RadioGroup
                defaultValue="candidate"
                className="grid grid-cols-2 gap-2"
              >
                <Label className="flex items-center gap-2 rounded-md border border-line p-3 text-sm has-[[data-state=checked]]:border-accent-jade has-[[data-state=checked]]:bg-accent-jade/5">
                  <RadioGroupItem value="candidate" />
                  {t("roleCandidate")}
                </Label>
                <Label className="flex items-center gap-2 rounded-md border border-line p-3 text-sm has-[[data-state=checked]]:border-accent-jade has-[[data-state=checked]]:bg-accent-jade/5">
                  <RadioGroupItem value="employer" />
                  {t("roleEmployer")}
                </Label>
              </RadioGroup>
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="email">{t("email")}</Label>
              <Input id="email" type="email" autoComplete="email" required />
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="phone">{t("phone")}</Label>
              <Input id="phone" type="tel" autoComplete="tel" />
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="password">{t("password")}</Label>
              <Input
                id="password"
                type="password"
                autoComplete="new-password"
                required
              />
            </div>
            <Button type="submit" className="w-full">
              {t("submitRegister")}
            </Button>
          </form>

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
