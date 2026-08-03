import { notFound } from "next/navigation";
import { getTranslations, getLocale } from "next-intl/server";
import { ShieldCheck, ShieldQuestion, MapPin, Building2 } from "lucide-react";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getOrganizationById, getOrganizationJobs } from "@/lib/api";

export default async function OrganizationPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const locale = await getLocale();
  const org = await getOrganizationById(id, locale);
  if (!org) notFound();

  const t = await getTranslations("organizations");
  const tJobs = await getTranslations("jobs");
  const jobs = await getOrganizationJobs(org.id, locale);
  const isVerified = org.verifyStatus === "Verified";

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-4xl px-4 py-10 sm:px-6">
          <div className="mb-8 flex flex-col gap-4 rounded-lg border border-line bg-paper-raised p-6 sm:flex-row sm:items-start sm:justify-between">
            <div className="flex items-start gap-4">
              <div className="flex size-12 shrink-0 items-center justify-center rounded-lg bg-paper-sunken text-ink-muted">
                <Building2 className="size-6" />
              </div>
              <div>
                <div className="mb-1 flex flex-wrap items-center gap-2">
                  {isVerified ? (
                    <Badge className="gap-1 bg-accent-jade text-white">
                      <ShieldCheck className="size-3" />
                      {t("verified")}
                    </Badge>
                  ) : (
                    <Badge
                      variant="outline"
                      className="gap-1 border-amber-pending/50 text-amber-pending"
                    >
                      <ShieldQuestion className="size-3" />
                      {t("notVerified")}
                    </Badge>
                  )}
                </div>
                <h1 className="font-heading text-2xl font-semibold text-ink">
                  {org.name}
                </h1>
                <p className="mt-1 flex items-center gap-1.5 text-sm text-ink-muted">
                  <MapPin className="size-3.5" />
                  {org.orgType}
                  {org.address ? ` · ${org.address}` : ""}
                </p>
              </div>
            </div>
          </div>

          <div className="grid gap-6 sm:grid-cols-3">
            <Card className="sm:col-span-2">
              <CardHeader>
                <CardTitle className="text-base">{t("aboutTitle")}</CardTitle>
              </CardHeader>
              <CardContent className="whitespace-pre-line text-sm leading-relaxed text-ink-muted">
                {org.description}
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardTitle className="text-base">{t("activeJobs")}</CardTitle>
              </CardHeader>
              <CardContent className="text-2xl font-semibold text-ink">
                {jobs.length}
              </CardContent>
            </Card>
          </div>

          <h2 className="mt-8 mb-4 font-heading text-xl font-semibold text-ink">
            {t("jobsTitle")}
          </h2>
          {jobs.length === 0 ? (
            <div className="rounded-lg border border-dashed border-line bg-paper-raised p-8 text-center text-ink-muted">
              {t("noJobs")}
            </div>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2">
              {jobs.map((job) => (
                <Link key={job.id} href={`/jobs/${job.id}`}>
                  <Card className="h-full transition-shadow hover:shadow-md">
                    <CardHeader>
                      <CardTitle className="text-base leading-snug">
                        {job.title}
                      </CardTitle>
                      <p className="text-sm text-ink-muted">
                        {job.specialtyName} · {job.locationName}
                      </p>
                    </CardHeader>
                    <CardContent className="flex items-center justify-between text-sm">
                      <span className="text-accent-jade">
                        {tJobs("applyNow")} →
                      </span>
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          )}
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
