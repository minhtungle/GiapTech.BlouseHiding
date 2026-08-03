import { getTranslations } from "next-intl/server";
import { FileText } from "lucide-react";
import { redirect } from "@/i18n/navigation";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { getCurrentUser } from "@/lib/session";
import { getMyCandidateProfile } from "@/lib/candidates";
import { ProfileForm } from "./profile-form";
import { LicenseSection } from "./license-section";

export default async function ProfilePage({
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

  const profile = await getMyCandidateProfile();

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
          <div className="mb-6 flex items-center justify-between">
            <h1 className="font-heading text-2xl font-semibold text-ink">
              {t("title")}
            </h1>
            <Button variant="outline" asChild>
              <Link href="/profile/cv">
                <FileText className="size-4" />
                {t("cvBuilderTitle")}
              </Link>
            </Button>
          </div>

          <AccountNav active="profile" />

          <div className="mt-6 grid gap-6 sm:grid-cols-2">
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Thông tin chung</CardTitle>
              </CardHeader>
              <CardContent>
                <ProfileForm profile={profile} />
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="text-base">
                  Chứng chỉ hành nghề (CCHN)
                </CardTitle>
              </CardHeader>
              <CardContent>
                <LicenseSection licenses={profile?.licenses ?? []} />
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
