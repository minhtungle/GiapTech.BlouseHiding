import { getTranslations } from "next-intl/server";
import { Plus, Download, GraduationCap, Briefcase, Sparkles } from "lucide-react";
import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AccountNav } from "@/components/account-nav";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { MOCK_CANDIDATE, MOCK_CV } from "@/lib/mock-data";

export default async function CvBuilderPage() {
  const t = await getTranslations("profile");

  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6">
          <div className="mb-6 flex items-center justify-between">
            <h1 className="font-heading text-2xl font-semibold text-ink">
              {t("cvBuilderTitle")}
            </h1>
            <Button variant="outline" disabled>
              <Download />
              {t("exportPdf")}
            </Button>
          </div>

          <AccountNav active="profile" />

          <div className="mt-6 grid gap-6 lg:grid-cols-[1fr_360px]">
            {/* Form chỉnh sửa */}
            <div className="space-y-6">
              <Card>
                <CardHeader className="flex flex-row items-center gap-2">
                  <GraduationCap className="size-4 text-accent-jade" />
                  <CardTitle className="text-base">{t("education")}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {MOCK_CV.education.map((edu) => (
                    <div
                      key={edu.id}
                      className="grid gap-3 rounded-md border border-line p-3 sm:grid-cols-2"
                    >
                      <div className="space-y-1.5">
                        <Label>{t("school")}</Label>
                        <Input defaultValue={edu.school} />
                      </div>
                      <div className="space-y-1.5">
                        <Label>{t("degree")}</Label>
                        <Input defaultValue={edu.degree} />
                      </div>
                      <div className="space-y-1.5 sm:col-span-2">
                        <Label>{t("period")}</Label>
                        <Input defaultValue={edu.period} />
                      </div>
                    </div>
                  ))}
                  <Button variant="outline" size="sm">
                    <Plus />
                    {t("addEntry")}
                  </Button>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center gap-2">
                  <Briefcase className="size-4 text-accent-jade" />
                  <CardTitle className="text-base">{t("experience")}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {MOCK_CV.experience.map((exp) => (
                    <div
                      key={exp.id}
                      className="grid gap-3 rounded-md border border-line p-3 sm:grid-cols-2"
                    >
                      <div className="space-y-1.5">
                        <Label>{t("employer")}</Label>
                        <Input defaultValue={exp.employer} />
                      </div>
                      <div className="space-y-1.5">
                        <Label>{t("role")}</Label>
                        <Input defaultValue={exp.role} />
                      </div>
                      <div className="space-y-1.5 sm:col-span-2">
                        <Label>{t("period")}</Label>
                        <Input defaultValue={exp.period} />
                      </div>
                    </div>
                  ))}
                  <Button variant="outline" size="sm">
                    <Plus />
                    {t("addEntry")}
                  </Button>
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center gap-2">
                  <Sparkles className="size-4 text-accent-jade" />
                  <CardTitle className="text-base">{t("skills")}</CardTitle>
                </CardHeader>
                <CardContent className="flex flex-wrap gap-2">
                  {MOCK_CV.skills.map((skill) => (
                    <Badge key={skill} variant="secondary">
                      {skill}
                    </Badge>
                  ))}
                  <Button variant="outline" size="sm">
                    <Plus />
                    {t("addEntry")}
                  </Button>
                </CardContent>
              </Card>

              <Button size="lg">{t("save")}</Button>
            </div>

            {/* Xem trước */}
            <div className="lg:sticky lg:top-20 lg:h-fit">
              <p className="mb-2 text-xs font-medium tracking-wide text-ink-muted uppercase">
                {t("cvPreview")}
              </p>
              <Card className="bg-paper-raised">
                <CardContent className="space-y-4 p-6 text-sm">
                  <div>
                    <p className="font-heading text-lg font-semibold text-ink">
                      {MOCK_CANDIDATE.fullName}
                    </p>
                    <p className="text-ink-muted">{MOCK_CANDIDATE.headline}</p>
                  </div>
                  <Separator />
                  <div>
                    <p className="mb-1 font-medium text-ink">{t("education")}</p>
                    {MOCK_CV.education.map((edu) => (
                      <div key={edu.id} className="text-ink-muted">
                        <p className="text-ink">{edu.school}</p>
                        <p>
                          {edu.degree} · {edu.period}
                        </p>
                      </div>
                    ))}
                  </div>
                  <Separator />
                  <div>
                    <p className="mb-1 font-medium text-ink">
                      {t("experience")}
                    </p>
                    {MOCK_CV.experience.map((exp) => (
                      <div key={exp.id} className="text-ink-muted">
                        <p className="text-ink">
                          {exp.role} — {exp.employer}
                        </p>
                        <p>{exp.period}</p>
                      </div>
                    ))}
                  </div>
                  <Separator />
                  <div>
                    <p className="mb-1 font-medium text-ink">{t("skills")}</p>
                    <div className="flex flex-wrap gap-1.5">
                      {MOCK_CV.skills.map((skill) => (
                        <Badge key={skill} variant="outline" className="text-xs">
                          {skill}
                        </Badge>
                      ))}
                    </div>
                  </div>
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
