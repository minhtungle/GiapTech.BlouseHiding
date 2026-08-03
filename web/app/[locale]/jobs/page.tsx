import { getTranslations, getLocale } from "next-intl/server";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { getSpecialties, getLocations } from "@/lib/api";
import { MOCK_SPECIALTIES, MOCK_LOCATIONS } from "@/lib/mock-data";
import { JobsBrowser } from "./jobs-browser";

export default async function JobsPage() {
  const t = await getTranslations("jobs");
  const locale = await getLocale();

  const [apiSpecialties, apiLocations] = await Promise.all([
    getSpecialties(locale),
    getLocations(locale),
  ]);

  // Fallback về mock nếu backend chưa chạy — chỉ để dev/demo frontend độc lập, không che giấu lỗi
  // thật (getSpecialties/getLocations tự log rỗng khi API lỗi, xem lib/api.ts).
  const specialtyNames =
    apiSpecialties.length > 0 ? apiSpecialties.map((s) => s.name) : MOCK_SPECIALTIES;
  const locationNames =
    apiLocations.length > 0 ? apiLocations.map((l) => l.name) : MOCK_LOCATIONS;

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
          <h1 className="mb-6 font-heading text-3xl font-semibold text-ink">
            {t("title")}
          </h1>
          <JobsBrowser specialties={specialtyNames} locations={locationNames} />
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
