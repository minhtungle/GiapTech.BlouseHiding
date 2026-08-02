import type { Metadata } from "next";
import { Be_Vietnam_Pro, Source_Serif_4 } from "next/font/google";
import { NextIntlClientProvider, hasLocale } from "next-intl";
import { getMessages, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";
import { routing, type Locale } from "@/i18n/routing";
import { Toaster } from "@/components/ui/sonner";
import "../globals.css";

// Be Vietnam Pro: tối ưu riêng cho dấu tiếng Việt (xem THIET-KE-GIAO-DIEN.md mục 2).
// next/font/google tự host tại build-time (không gọi runtime tới Google Fonts).
const bodyFont = Be_Vietnam_Pro({
  variable: "--font-body",
  subsets: ["latin", "vietnamese"],
  weight: ["400", "500", "600", "700"],
});

// Serif tiêu đề — dùng tiết chế (chỉ H1/H2), cần soát thủ công glyph tiếng Việt trước khi chốt cuối.
const displayFont = Source_Serif_4({
  variable: "--font-display",
  subsets: ["latin", "vietnamese"],
  weight: ["600", "700"],
});

export const metadata: Metadata = {
  title: "BlouseHiding — Tuyển dụng ngành y tế",
  description: "Nền tảng tuyển dụng chuyên biệt cho ngành y tế tại Việt Nam",
};

export function generateStaticParams() {
  return routing.locales.map((locale) => ({ locale }));
}

export default async function LocaleLayout({
  children,
  params,
}: {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) {
    notFound();
  }

  // Cho phép render tĩnh (generateStaticParams) dù đọc locale động — xem next-intl docs.
  setRequestLocale(locale as Locale);

  const messages = await getMessages();

  return (
    <html
      lang={locale}
      className={`${bodyFont.variable} ${displayFont.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col font-sans">
        <NextIntlClientProvider messages={messages}>
          {children}
          <Toaster />
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
