type StatCardProps = {
  label: string;
  value: string;
  hint?: string;
};

export function StatCard({ label, value, hint }: StatCardProps) {
  return (
    <article className="stat-card">
      <p className="stat-label">{label}</p>
      <h3 className="stat-value">{value}</h3>
      {hint ? <p className="stat-hint">{hint}</p> : null}
    </article>
  );
}
