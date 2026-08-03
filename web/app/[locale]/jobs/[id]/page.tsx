import { notFound } from "next/navigation";
import { getTranslations, getLocale } from "next-intl/server";
import {
  ShieldCheck,
  Stethoscope,
  MapPin,
  Briefcase,
  Wallet,
  ChevronLeft,
} from "lucide-react";
import { Link } from "@/i18n/navigation";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { getJobById } from "@/lib/api";
import { getCurrentUser } from "@/lib/session";
import { ApplyButton } from "./apply-button";

function formatSalary(job: { salaryMin: number | null; salaryMax: number | null; salaryNegotiable: boolean }) {
  if (job.salaryNegotiable) return "Thỏa thuận";
  if (!job.salaryMin && !job.salaryMax) return "Thỏa thuận";
  const format = (n: number) => `${(n / 1_000_000).toFixed(0)} triệu`;
  if (job.salaryMin && job.salaryMax) return `${format(job.salaryMin)} - ${format(job.salaryMax)}`;
  return format((job.salaryMin ?? job.salaryMax)!);
}

export default async function JobDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const locale = await getLocale();
  const job = await getJobById(id, locale);
  if (!job) notFound();

  const t = await getTranslations("jobs");
  const currentUser = await getCurrentUser();

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-4xl px-4 py-10 sm:px-6">
          <Link
            href="/jobs"
            className="mb-6 inline-flex items-center gap-1 text-sm text-ink-muted hover:text-ink"
          >
            <ChevronLeft className="size-4" />
            {t("backToList")}
          </Link>

          <div className="mb-6 flex flex-col gap-4 rounded-lg border border-line bg-paper-raised p-6 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <div className="mb-2 flex flex-wrap items-center gap-2">
                {job.requiredLicense && (
                  <Badge
                    variant="outline"
                    className="gap-1 border-accent-jade/40 text-accent-jade"
                  >
                    <ShieldCheck className="size-3" />
                    {t("requireLicense")}
                  </Badge>
                )}
              </div>
              <h1 className="font-heading text-2xl font-semibold text-ink sm:text-3xl">
                {job.title}
              </h1>
              <p className="mt-1 text-ink-muted">{job.organizationName}</p>

              <div className="mt-4 flex flex-wrap gap-x-5 gap-y-2 text-sm text-ink-muted">
                <span className="inline-flex items-center gap-1.5">
                  <Stethoscope className="size-3.5" />
                  {job.specialtyName}
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <MapPin className="size-3.5" />
                  {job.locationName}
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <Briefcase className="size-3.5" />
                  {job.employmentType}
                </span>
                <span className="inline-flex items-center gap-1.5 font-medium text-ink">
                  <Wallet className="size-3.5" />
                  {formatSalary(job)}
                </span>
              </div>
            </div>

            <ApplyButton jobId={job.id} isLoggedIn={currentUser !== null} />
          </div>

          <div className="grid gap-6 sm:grid-cols-3">
            <div className="space-y-6 sm:col-span-2">
              <Card>
                <CardHeader>
                  <CardTitle>{t("jobDescription")}</CardTitle>
                </CardHeader>
                <CardContent className="whitespace-pre-line text-sm leading-relaxed text-ink-muted">
                  {job.description}
                </CardContent>
              </Card>

              {job.requirements && (
                <Card>
                  <CardHeader>
                    <CardTitle>{t("requirements")}</CardTitle>
                  </CardHeader>
                  <CardContent className="whitespace-pre-line text-sm leading-relaxed text-ink-muted">
                    {job.requirements}
                  </CardContent>
                </Card>
              )}

              {job.benefits && (
                <Card>
                  <CardHeader>
                    <CardTitle>{t("benefits")}</CardTitle>
                  </CardHeader>
                  <CardContent className="whitespace-pre-line text-sm leading-relaxed text-ink-muted">
                    {job.benefits}
                  </CardContent>
                </Card>
              )}
            </div>

            <div>
              <Card>
                <CardHeader>
                  <CardTitle className="text-base">{t("aboutOrg")}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-3 text-sm text-ink-muted">
                  <p className="font-medium text-ink">
                    {job.organizationName}
                  </p>
                  <p>{job.locationName}</p>
                  <Separator />
                  <Link
                    href={`/organizations/${job.organizationId}`}
                    className="text-accent-jade hover:underline"
                  >
                    Xem trang tổ chức →
                  </Link>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
