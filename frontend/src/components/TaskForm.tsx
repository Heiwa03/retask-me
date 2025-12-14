import { FormEvent, useState } from "react";
import "./TaskForm.css";

interface Props {
  onCreate: (data: {
    title: string;
    description?: string;
    deadline?: string;
    priority: number;
    status: number;
  }) => Promise<void>;
}

const priorities = [
  { label: "Low", value: 1 },
  { label: "Medium", value: 2 },
  { label: "High", value: 3 },
];

const statuses = [
  { label: "Pending", value: 0 },
  { label: "New", value: 1 },
  { label: "In Progress", value: 2 },
  { label: "Done", value: 3 },
];

export default function TaskForm({ onCreate }: Props) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [deadline, setDeadline] = useState("");
  const [priority, setPriority] = useState(2);
  const [status, setStatus] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await onCreate({
        title,
        description,
        deadline: deadline || undefined,
        priority,
        status,
      });
      setTitle("");
      setDescription("");
      setDeadline("");
      setPriority(2);
      setStatus(1);
    } catch (err: any) {
      setError(err?.message ?? "Failed to create task");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <div className="task-form__row">
        <div>
          <label className="label">Title</label>
          <input
            className="input"
            required
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Design landing page"
          />
        </div>
        <div>
          <label className="label">Deadline</label>
          <input
            className="input"
            type="date"
            value={deadline}
            onChange={(e) => setDeadline(e.target.value)}
          />
        </div>
      </div>

      <div>
        <label className="label">Description</label>
        <textarea
            className="input"
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional notes"
        />
      </div>

      <div className="task-form__row">
        <div>
          <label className="label">Priority</label>
          <select
            className="input"
            value={priority}
            onChange={(e) => setPriority(Number(e.target.value))}
          >
            {priorities.map((p) => (
              <option key={p.value} value={p.value}>
                {p.label}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="label">Status</label>
          <select
            className="input"
            value={status}
            onChange={(e) => setStatus(Number(e.target.value))}
          >
            {statuses.map((s) => (
              <option key={s.value} value={s.value}>
                {s.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      {error && <div className="error">{error}</div>}

      <button className="primary-btn" type="submit" disabled={loading}>
        {loading ? "Saving..." : "Add task"}
      </button>
    </form>
  );
}

