import { useQuery } from "@tanstack/react-query";
import { apiClient } from "../../core/http/apiClient";
import { getApiErrorMessage } from "../../core/http/errorMessage";
import { PageHeader } from "../../shared/components/PageHeader";

type SiteItem = {
  id?: string;
  siteCode?: string;
  name?: string;
  status?: string;
  officeCode?: string;
  region?: string;
};

async function fetchMaintenanceSites() {
  const response = await apiClient.get<SiteItem[] | { items?: SiteItem[] }>("/sites/maintenance");
  const payload = response.data;
  if (Array.isArray(payload)) {
    return payload;
  }

  return payload.items ?? [];
}

export function SitesPage() {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["sites", "maintenance"],
    queryFn: fetchMaintenanceSites,
  });

  return (
    <section>
      <PageHeader
        title="Sites in Maintenance Scope"
        description="Operational site records used for visit and work-order execution."
      />

      {isLoading ? <p className="status-info">Loading sites...</p> : null}
      {isError ? (
        <p className="status-error">{getApiErrorMessage(error, "Unable to load sites.")}</p>
      ) : null}

      {!isLoading && !isError ? (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Site Code</th>
                <th>Name</th>
                <th>Status</th>
                <th>Office</th>
                <th>Region</th>
              </tr>
            </thead>
            <tbody>
              {(data ?? []).map((site, index) => (
                <tr key={site.id ?? site.siteCode ?? `site-${index}`}>
                  <td>{site.siteCode ?? "-"}</td>
                  <td>{site.name ?? "-"}</td>
                  <td>{site.status ?? "-"}</td>
                  <td>{site.officeCode ?? "-"}</td>
                  <td>{site.region ?? "-"}</td>
                </tr>
              ))}
              {(data ?? []).length === 0 ? (
                <tr>
                  <td colSpan={5} className="table-empty">
                    No sites returned for this account.
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
