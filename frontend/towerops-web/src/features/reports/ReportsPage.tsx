import { PageHeader } from "../../shared/components/PageHeader";

const reportEndpoints = [
  {
    label: "Checklist Export",
    endpoint: "/api/reports/checklist",
    description: "Export checklist workbook in GH-DE format.",
  },
  {
    label: "Battery Discharge Test Export",
    endpoint: "/api/reports/bdt",
    description: "Generate BDT output workbook.",
  },
  {
    label: "Data Collection Export",
    endpoint: "/api/reports/data-collection",
    description: "Generate full field data collection workbook.",
  },
  {
    label: "Monthly Scorecard",
    endpoint: "/api/reports/scorecard",
    description: "Download office-level monthly scorecard.",
  },
];

export function ReportsPage() {
  return (
    <section>
      <PageHeader
        title="Reports and Export"
        description="Download operational exports aligned to field templates."
      />

      <div className="report-grid">
        {reportEndpoints.map((report) => (
          <article key={report.endpoint} className="report-card">
            <h3>{report.label}</h3>
            <p>{report.description}</p>
            <code>{report.endpoint}</code>
          </article>
        ))}
      </div>
    </section>
  );
}
