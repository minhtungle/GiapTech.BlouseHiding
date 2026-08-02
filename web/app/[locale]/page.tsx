import { getTranslations } from "next-intl/server";
import { ShieldCheck, Stethoscope, MapPin, Search } from "lucide-react";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { MOCK_JOBS, MOCK_ORGANIZATIONS } from "@/lib/mock-data";

export default async function HomePage() {
  const t = await getTranslations("home");
  const tJobs = await getTranslations("jobs");

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        {/* Hero + tìm kiếm nhanh */}
        <section className="border-b border-border bg-paper">
          <div className="mx-auto max-w-6xl px-4 py-16 sm:px-6 sm:py-24">
            <div className="mb-3 inline-flex items-center gap-2 rounded-full border border-line bg-paper-raised px-3 py-1 text-xs font-medium text-accent-jade">
              <ShieldCheck className="size-3.5" />
              Xác thực chứng chỉ hành nghề (CCHN)
            </div>
            <h1 className="max-w-2xl font-heading text-4xl font-semibold leading-tight text-ink sm:text-5xl">
              {t("heroTitle")}
            </h1>
            <p className="mt-4 max-w-xl text-lg text-ink-muted">
              {t("heroSubtitle")}
            </p>

            <form className="mt-8 flex max-w-2xl flex-col gap-2 rounded-lg border border-line bg-paper-raised p-2 sm:flex-row">
              <div className="flex flex-1 items-center gap-2 px-3">
                <Search className="size-4 shrink-0 text-ink-muted" />
                <Input
                  placeholder={t("searchPlaceholder")}
                  className="border-0 shadow-none focus-visible:ring-0"
                />
              </div>
              <Button type="submit" className="sm:w-auto">
                {t("ctaSearch")}
              </Button>
            </form>
          </div>
        </section>

        {/* Tin tuyển dụng nổi bật */}
        <section className="mx-auto max-w-6xl px-4 py-16 sm:px-6">
          <div className="mb-6 flex items-center justify-between">
            <h2 className="font-heading text-2xl font-semibold text-ink">
              {t("featuredJobs")}
            </h2>
          </div>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {MOCK_JOBS.map((job) => (
              <Link key={job.id} href={`/jobs/${job.id}`}>
                <Card className="h-full transition-shadow hover:shadow-md">
                  <CardHeader>
                    <div className="flex items-start justify-between gap-2">
                      <CardTitle className="text-base leading-snug">
                        {job.title}
                      </CardTitle>
                      {job.requiresLicense && (
                        <Badge
                          variant="outline"
                          className="shrink-0 gap-1 border-accent-jade/40 text-accent-jade"
                        >
                          <ShieldCheck className="size-3" />
                          CCHN
                        </Badge>
                      )}
                    </div>
                    <p className="text-sm text-ink-muted">
                      {job.organizationName}
                    </p>
                  </CardHeader>
                  <CardContent className="flex flex-col gap-2 text-sm text-ink-muted">
                    <span className="inline-flex items-center gap-1.5">
                      <Stethoscope className="size-3.5" />
                      {job.specialty}
                    </span>
                    <span className="inline-flex items-center gap-1.5">
                      <MapPin className="size-3.5" />
                      {job.location} · {job.employmentType}
                    </span>
                    <div className="mt-1 flex items-center justify-between">
                      <span className="font-medium text-ink">
                        {job.salaryLabel}
                      </span>
                      <span className="text-accent-jade">
                        {tJobs("applyNow")} →
                      </span>
                    </div>
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        </section>

        {/* Cơ sở y tế nổi bật */}
        <section className="mx-auto max-w-6xl px-4 pb-20 sm:px-6">
          <h2 className="mb-6 font-heading text-2xl font-semibold text-ink">
            {t("featuredOrgs")}
          </h2>
          <div className="grid gap-4 sm:grid-cols-3">
            {MOCK_ORGANIZATIONS.map((org) => (
              <Link key={org.id} href={`/organizations/${org.id}`}>
                <Card className="h-full transition-shadow hover:shadow-md">
                  <CardHeader>
                    <div className="flex items-center justify-between gap-2">
                      <CardTitle className="text-base">{org.name}</CardTitle>
                      {org.verified && (
                        <Badge className="gap-1 bg-accent-jade text-white">
                          <ShieldCheck className="size-3" />
                          Đã xác thực
                        </Badge>
                      )}
                    </div>
                  </CardHeader>
                  <CardContent className="text-sm text-ink-muted">
                    {org.type} · {org.location}
                    <div className="mt-1">{org.activeJobs} tin đang tuyển</div>
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        </section>
      </main>
      <SiteFooter />
    </>
  );
}
