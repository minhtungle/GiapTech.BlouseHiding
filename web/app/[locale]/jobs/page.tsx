import { getTranslations } from "next-intl/server";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { JobsBrowser } from "./jobs-browser";

export default async function JobsPage() {
  const t = await getTranslations("jobs");

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6">
          <h1 className="mb-6 font-heading text-3xl font-semibold text-ink">
            {t("title")}
          </h1>
          <JobsBrowser />
        </div>
      </main>
      <SiteFooter />
    </>
  );
}
