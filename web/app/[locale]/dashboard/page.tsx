import { getTranslations } from "next-intl/server";
import { ShieldCheck, ShieldQuestion, Briefcase, Bookmark } from "lucide-react";
import { redirect } from "@/i18n/navigation";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Progress } from "@/components/ui/progress";
import { getCurrentUser } from "@/lib/session";
import { getMyCandidateProfile, getMyApplications } from "@/lib/candidates";

const STAGE_VARIANT: Record<string, "default" | "outline" | "secondary"> = {
  New: "secondary",
  Reviewing: "outline",
  Shortlisted: "outline",
  Interview: "default",
  Offer: "default",
  Hired: "default",
  Rejected: "secondary",
};

export default async function DashboardPage({
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

  const [profile, applications] = await Promise.all([
    getMyCandidateProfile(),
    getMyApplications(),
  ]);

  const hasVerifiedLicense = profile?.licenses.some((l) => l.verifyStatus === "Verified") ?? false;

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
          <h1 className="mb-1 font-heading text-2xl font-semibold text-ink">
            {t("welcomeBack")}
            {profile ? `, ${profile.fullName}` : ""}
          </h1>
          <p className="mb-6 text-ink-muted">{profile?.headline}</p>

          <AccountNav active="dashboard" />

          <div className="mt-6 grid gap-4 sm:grid-cols-3">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-ink-muted">
                  {t("completion")}
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="mb-2 text-2xl font-semibold text-ink">
                  {profile?.completionPct ?? 0}%
                </div>
                <Progress value={profile?.completionPct ?? 0} />
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-ink-muted">
                  {t("tabApplications")}
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center gap-2 text-2xl font-semibold text-ink">
                  <Briefcase className="size-5 text-accent-jade" />
                  {applications.length}
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-ink-muted">
                  CCHN
                </CardTitle>
              </CardHeader>
              <CardContent>
                {hasVerifiedLicense ? (
                  <Badge className="gap-1 bg-accent-jade text-white">
                    <ShieldCheck className="size-3" />
                    {t("verifyStatusVerified")}
                  </Badge>
                ) : (
                  <Badge variant="outline" className="gap-1">
                    <ShieldQuestion className="size-3" />
                    {t("verifyStatusPending")}
                  </Badge>
                )}
              </CardContent>
            </Card>
          </div>

          <Card className="mt-6">
            <CardHeader>
              <CardTitle className="text-base">
                {t("tabApplications")}
              </CardTitle>
            </CardHeader>
            <CardContent className="divide-y divide-border">
              {applications.length === 0 ? (
                <p className="py-3 text-sm text-ink-muted">
                  Chưa có đơn ứng tuyển nào.
                </p>
              ) : (
                applications.map((app) => (
                  <div
                    key={app.id}
                    className="flex items-center justify-between gap-4 py-3 first:pt-0 last:pb-0"
                  >
                    <div>
                      <Link
                        href={`/jobs/${app.jobId}`}
                        className="font-medium text-ink hover:text-accent-jade"
                      >
                        {app.jobTitle}
                      </Link>
                    </div>
                    <Badge variant={STAGE_VARIANT[app.stage] ?? "secondary"}>
                      {t(`stage.${app.stage}`)}
                    </Badge>
                  </div>
                ))
              )}
            </CardContent>
          </Card>

          <div className="mt-6 flex items-center gap-2 text-sm text-ink-muted">
            <Bookmark className="size-4" />
            Danh sách việc đã lưu — chưa có mục nào
          </div>
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
