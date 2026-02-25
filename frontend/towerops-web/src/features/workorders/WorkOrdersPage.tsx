import { useState, type FormEvent } from "react";
import { apiClient } from "../../core/http/apiClient";
import { getApiErrorMessage } from "../../core/http/errorMessage";
import { PageHeader } from "../../shared/components/PageHeader";

type WorkOrderDto = {
  id?: string;
  workOrderId?: string;
  siteCode?: string;
  status?: string;
  slaClass?: string;
  createdAtUtc?: string;
  closedAtUtc?: string;
};

export function WorkOrdersPage() {
  const [workOrderId, setWorkOrderId] = useState("");
  const [result, setResult] = useState<WorkOrderDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function lookupWorkOrder(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setResult(null);

    if (!workOrderId.trim()) {
      setError("Enter a Work Order ID.");
      return;
    }

    try {
      setBusy(true);
      const response = await apiClient.get<WorkOrderDto>(`/workorders/${workOrderId.trim()}`);
      setResult(response.data);
    } catch (lookupError) {
      setError(getApiErrorMessage(lookupError, "Unable to fetch work order."));
    } finally {
      setBusy(false);
    }
  }

  return (
    <section>
      <PageHeader
        title="Work Order Lookup"
        description="Use direct ID lookup to inspect lifecycle status and SLA metadata."
      />

      <form className="lookup-form" onSubmit={lookupWorkOrder}>
        <input
          type="text"
          placeholder="Enter WorkOrder ID (GUID)"
          value={workOrderId}
          onChange={(event) => setWorkOrderId(event.target.value)}
        />
        <button type="submit" disabled={busy}>
          {busy ? "Searching..." : "Search"}
        </button>
      </form>

      {error ? <p className="status-error">{error}</p> : null}

      {result ? (
        <div className="json-card">
          <pre>{JSON.stringify(result, null, 2)}</pre>
        </div>
      ) : null}
    </section>
  );
}
