import { getTranslations } from "next-intl/server";
import { ShieldCheck, Briefcase, Bookmark } from "lucide-react";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Progress } from "@/components/ui/progress";
import { MOCK_CANDIDATE, MOCK_APPLICATIONS } from "@/lib/mock-data";

const STAGE_VARIANT: Record<string, "default" | "outline" | "secondary"> = {
  moi: "secondary",
  dang_xem: "outline",
  phu_hop: "outline",
  phong_van: "default",
  offer: "default",
  trung_tuyen: "default",
  tu_choi: "secondary",
};

export default async function DashboardPage() {
  const t = await getTranslations("profile");

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
          <h1 className="mb-1 font-heading text-2xl font-semibold text-ink">
            {t("welcomeBack")}, {MOCK_CANDIDATE.fullName}
          </h1>
          <p className="mb-6 text-ink-muted">{MOCK_CANDIDATE.headline}</p>

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
                  {MOCK_CANDIDATE.completionPercent}%
                </div>
                <Progress value={MOCK_CANDIDATE.completionPercent} />
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
                  {MOCK_APPLICATIONS.length}
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
                <Badge className="gap-1 bg-accent-jade text-white">
                  <ShieldCheck className="size-3" />
                  {t("verifyStatusVerified")}
                </Badge>
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
              {MOCK_APPLICATIONS.map((app) => (
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
                    <p className="text-sm text-ink-muted">
                      {app.organizationName}
                    </p>
                  </div>
                  <Badge variant={STAGE_VARIANT[app.stage]}>
                    {t(`stage.${app.stage}`)}
                  </Badge>
                </div>
              ))}
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
