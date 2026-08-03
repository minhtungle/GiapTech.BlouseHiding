import { getTranslations } from "next-intl/server";
import { AlertTriangle } from "lucide-react";
import { redirect } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { LanguageSwitcher } from "@/components/language-switcher";
import { getCurrentUser } from "@/lib/session";
import { DeleteAccountButton } from "./delete-account-button";

export default async function SettingsPage({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations("profile");
  const currentUser = await getCurrentUser();

  if (!currentUser) {
    redirect({ href: "/auth/login", locale });
  }

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
          <h1 className="mb-6 font-heading text-2xl font-semibold text-ink">
            {t("settingsTitle")}
          </h1>

          <AccountNav active="settings" />

          <div className="mt-6 space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="text-base">{t("changePassword")}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="current-password">Mật khẩu hiện tại</Label>
                  <Input id="current-password" type="password" />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="new-password">Mật khẩu mới</Label>
                  <Input id="new-password" type="password" />
                </div>
                <Button>{t("changePassword")}</Button>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-base">Ngôn ngữ</CardTitle>
              </CardHeader>
              <CardContent className="flex items-center justify-between">
                <p className="text-sm text-ink-muted">
                  Ngôn ngữ hiển thị hiện tại
                </p>
                <LanguageSwitcher />
              </CardContent>
            </Card>

            <Card className="border-accent-seal/30">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-base text-accent-seal">
                  <AlertTriangle className="size-4" />
                  {t("deleteAccount")}
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <p className="text-sm text-ink-muted">
                  {t("deleteAccountWarning")}
                </p>
                <DeleteAccountButton />
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
