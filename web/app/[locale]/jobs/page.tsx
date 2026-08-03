import { getTranslations, getLocale } from "next-intl/server";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { getSpecialties, getLocations, getJobs, getEmploymentTypes } from "@/lib/api";
import { JobsBrowser } from "./jobs-browser";

export default async function JobsPage({
  searchParams,
}: {
  searchParams: Promise<{ specialty?: string; location?: string; employmentType?: string }>;
}) {
  const t = await getTranslations("jobs");
  const locale = await getLocale();
  const params = await searchParams;

  const [specialties, locations, employmentTypes, jobs] = await Promise.all([
    getSpecialties(locale),
    getLocations(locale),
    getEmploymentTypes(locale),
    getJobs(locale, {
      specialty: params.specialty,
      location: params.location,
      employmentType: params.employmentType,
    }),
  ]);

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
          <h1 className="mb-6 font-heading text-3xl font-semibold text-ink">
            {t("title")}
          </h1>
          <JobsBrowser
            specialties={specialties}
            locations={locations}
            employmentTypes={employmentTypes}
            jobs={jobs}
          />
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
