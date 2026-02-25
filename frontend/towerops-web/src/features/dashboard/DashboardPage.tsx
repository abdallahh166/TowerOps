import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { apiClient } from "../../core/http/apiClient";
import { getApiErrorMessage } from "../../core/http/errorMessage";
import { PageHeader } from "../../shared/components/PageHeader";
import { StatCard } from "../../shared/components/StatCard";

type OperationsDashboardDto = {
  slaCompliancePercent?: number;
  ftfRatePercent?: number;
  mttrHours?: number;
  reopenRatePercent?: number;
  evidenceCompletenessPercent?: number;
  totalOpenWorkOrders?: number;
  totalEscalations?: number;
};

async function fetchOperationsDashboard() {
  const toDateUtc = new Date();
  const fromDateUtc = new Date();
  fromDateUtc.setDate(toDateUtc.getDate() - 30);

  const response = await apiClient.get<OperationsDashboardDto>("/kpi/operations", {
    params: {
      fromDateUtc: fromDateUtc.toISOString(),
      toDateUtc: toDateUtc.toISOString(),
    },
  });

  return response.data;
}

function formatPercent(value?: number) {
  return `${(value ?? 0).toFixed(1)}%`;
}

function formatHours(value?: number) {
  return `${(value ?? 0).toFixed(2)} h`;
}

export function DashboardPage() {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["kpi", "operations"],
    queryFn: fetchOperationsDashboard,
  });

  const summary = useMemo(
    () => [
      {
        label: "SLA Compliance",
        value: formatPercent(data?.slaCompliancePercent),
        hint: "P1-P4 compliance ratio",
      },
      {
        label: "First-Time Fix",
        value: formatPercent(data?.ftfRatePercent),
        hint: "Closed without rework/reopen",
      },
      {
        label: "MTTR",
        value: formatHours(data?.mttrHours),
        hint: "Average repair duration",
      },
      {
        label: "Reopen Rate",
        value: formatPercent(data?.reopenRatePercent),
        hint: "Closed work orders reopened",
      },
      {
        label: "Evidence Completeness",
        value: formatPercent(data?.evidenceCompletenessPercent),
        hint: "Submitted visits with full evidence",
      },
      {
        label: "Open Work Orders",
        value: String(data?.totalOpenWorkOrders ?? 0),
        hint: "Current active backlog",
      },
      {
        label: "Escalations",
        value: String(data?.totalEscalations ?? 0),
        hint: "Current escalation volume",
      },
    ],
    [data],
  );

  return (
    <section>
      <PageHeader
        title="Operations Dashboard"
        description="Live KPI snapshot for the last 30 days."
      />

      {isLoading ? <p className="status-info">Loading KPI data...</p> : null}

      {isError ? (
        <p className="status-error">
          {getApiErrorMessage(error, "Failed to load dashboard metrics.")}
        </p>
      ) : null}

      {!isLoading && !isError ? (
        <div className="stats-grid">
          {summary.map((item) => (
            <StatCard key={item.label} label={item.label} value={item.value} hint={item.hint} />
          ))}
        </div>
      ) : null}
    </section>
  );
}
