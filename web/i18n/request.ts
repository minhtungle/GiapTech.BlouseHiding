import { getRequestConfig } from "next-intl/server";
import { hasLocale } from "next-intl";
import { routing } from "./routing";

// Namespace tách theo tính năng, không theo route group (chỉ còn Client dùng app này — ADR-0008).
const NAMESPACES = [
  "common",
  "home",
  "jobs",
  "auth",
  "profile",
  "organizations",
] as const;

export default getRequestConfig(async ({ requestLocale }) => {
  const requested = await requestLocale;
  const locale = hasLocale(routing.locales, requested)
    ? requested
    : routing.defaultLocale;

  const messages = Object.fromEntries(
    await Promise.all(
      NAMESPACES.map(async (ns) => [
        ns,
        (await import(`../messages/${locale}/${ns}.json`)).default,
      ]),
    ),
  );

  return { locale, messages };
});
