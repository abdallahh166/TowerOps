import { useQuery } from "@tanstack/react-query";
import { apiClient } from "../../core/http/apiClient";
import { getApiErrorMessage } from "../../core/http/errorMessage";
import { PageHeader } from "../../shared/components/PageHeader";

type VisitItem = {
  id?: string;
  visitId?: string;
  siteCode?: string;
  status?: string;
  scheduledDateUtc?: string;
  engineerId?: string;
};

async function fetchScheduledVisits() {
  const response = await apiClient.get<VisitItem[] | { items?: VisitItem[] }>("/visits/scheduled");
  const payload = response.data;
  if (Array.isArray(payload)) {
    return payload;
  }

  return payload.items ?? [];
}

export function VisitsPage() {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["visits", "scheduled"],
    queryFn: fetchScheduledVisits,
  });

  return (
    <section>
      <PageHeader
        title="Scheduled Visits"
        description="Upcoming site visits ready for execution."
      />

      {isLoading ? <p className="status-info">Loading scheduled visits...</p> : null}
      {isError ? (
        <p className="status-error">
          {getApiErrorMessage(error, "Unable to load scheduled visits.")}
        </p>
      ) : null}

      {!isLoading && !isError ? (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Visit ID</th>
                <th>Site</th>
                <th>Status</th>
                <th>Scheduled</th>
                <th>Engineer</th>
              </tr>
            </thead>
            <tbody>
              {(data ?? []).map((visit, index) => (
                <tr key={visit.id ?? visit.visitId ?? `visit-${index}`}>
                  <td>{visit.id ?? visit.visitId ?? "-"}</td>
                  <td>{visit.siteCode ?? "-"}</td>
                  <td>{visit.status ?? "-"}</td>
                  <td>
                    {visit.scheduledDateUtc
                      ? new Date(visit.scheduledDateUtc).toLocaleString()
                      : "-"}
                  </td>
                  <td>{visit.engineerId ?? "-"}</td>
                </tr>
              ))}
              {(data ?? []).length === 0 ? (
                <tr>
                  <td colSpan={5} className="table-empty">
                    No scheduled visits returned for this account.
                  </td>
                </tr>
              ) : null}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  );
}
