import { useMemo, useState } from "react";
import { Task } from "../types";
import "./TaskList.css";

interface Props {
  tasks: Task[];
  onEdit: (taskUid: string, update: { title: string; description?: string; deadline?: string | null; priority: number; status: number }) => Promise<void>;
  onDelete: (taskUid: string) => Promise<void>;
  onMarkComplete: (taskUid: string) => Promise<void>;
  boards: { uuid: string; title: string }[];
  onAssignBoard: (taskUid: string, boardUuid: string) => Promise<void>;
  onRemoveBoard: (taskUid: string) => Promise<void>;
}

const priorityLabels: Record<number, string> = {
  1: "Low",
  2: "Medium",
  3: "High",
};

const statusLabels: Record<number, string> = {
  0: "Pending",
  1: "New",
  2: "InProgress",
  3: "Done",
  4: "Archived",
};

export default function TaskList({ tasks, onEdit, onDelete, onMarkComplete, boards, onAssignBoard, onRemoveBoard }: Props) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState<{
    title: string;
    description?: string | null;
    deadline?: string | null;
    priority: number;
    status: number;
  }>({
    title: "",
    description: "",
    deadline: "",
    priority: 2,
    status: 1,
  });

  const taskMap = useMemo(() => Object.fromEntries(tasks.map(t => [t.uuid, t])), [tasks]);

  const startEdit = (task: Task) => {
    setEditingId(task.uuid);
    setForm({
      title: task.title,
      description: task.description ?? "",
      deadline: task.deadline ?? "",
      priority: task.priority,
      status: task.status,
    });
  };

  const cancelEdit = () => {
    setEditingId(null);
  };

  const handleSave = async (taskUid: string) => {
    setSaving(true);
    try {
      await onEdit(taskUid, {
        title: form.title,
        description: form.description ?? "",
        deadline: form.deadline || null,
        priority: form.priority,
        status: form.status,
      });
      setEditingId(null);
    } finally {
      setSaving(false);
    }
  };

  const handleMarkComplete = async (taskUid: string) => {
    setSaving(true);
    try {
      await onMarkComplete(taskUid);
    } finally {
      setSaving(false);
    }
  };

  const handleBoardChange = async (taskUid: string, value: string) => {
    setSaving(true);
    try {
      if (!value) {
        await onRemoveBoard(taskUid);
      } else {
        await onAssignBoard(taskUid, value);
      }
    } finally {
      setSaving(false);
    }
  };

  if (!tasks.length) {
    return (
      <div className="task-list empty-card">
        <p>No tasks yet. Create one or ask the agent to help.</p>
      </div>
    );
  }

  return (
    <div className="task-list">
      {tasks.map((task) => (
        <div key={task.uuid} className="task-card">
          <div className="task-card__row">
            <h3>{task.title}</h3>
            <span className={`pill pill--${priorityLabels[task.priority]?.toLowerCase() ?? "low"}`}>
              {priorityLabels[task.priority] ?? "Unknown"}
            </span>
          </div>
          {task.description && <p className="task-card__desc">{task.description}</p>}
          {editingId === task.uuid && (
            <div className="task-edit">
              <div className="task-edit__row">
                <div>
                  <label className="label">Title</label>
                  <input
                    className="input"
                    value={form.title}
                    onChange={(e) => setForm((prev) => ({ ...prev, title: e.target.value }))}
                  />
                </div>
                <div>
                  <label className="label">Deadline</label>
                  <input
                    className="input"
                    type="date"
                    value={form.deadline ? form.deadline.split("T")[0] : ""}
                    onChange={(e) => setForm((prev) => ({ ...prev, deadline: e.target.value || null }))}
                  />
                </div>
              </div>
              <div>
                <label className="label">Description</label>
                <textarea
                  className="input"
                  rows={3}
                  value={form.description ?? ""}
                  onChange={(e) => setForm((prev) => ({ ...prev, description: e.target.value }))}
                />
              </div>
              <div className="task-edit__row">
                <div>
                  <label className="label">Priority</label>
                  <select
                    className="input"
                    value={form.priority}
                    onChange={(e) => setForm((prev) => ({ ...prev, priority: Number(e.target.value) }))}
                  >
                    {Object.entries(priorityLabels).map(([value, label]) => (
                      <option key={value} value={value}>{label}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="label">Status</label>
                  <select
                    className="input"
                    value={form.status}
                    onChange={(e) => setForm((prev) => ({ ...prev, status: Number(e.target.value) }))}
                  >
                    {Object.entries(statusLabels).map(([value, label]) => (
                      <option key={value} value={value}>{label}</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="task-actions">
                <button className="primary-btn" type="button" disabled={saving} onClick={() => handleSave(task.uuid)}>
                  {saving ? "Saving..." : "Save"}
                </button>
                <button className="secondary-btn" type="button" disabled={saving} onClick={cancelEdit}>
                  Cancel
                </button>
              </div>
            </div>
          )}
          <div className="task-card__meta">
            <span className={`status status--${statusLabels[task.status]?.toLowerCase() ?? "pending"}`}>
              {statusLabels[task.status] ?? "Unknown"}
            </span>
            {task.deadline && (
              <span className="deadline">
                Due {new Date(task.deadline).toLocaleDateString()}
              </span>
            )}
            <span className="board-chip">
              Board:
              <select
                className="input board-select"
                value={task.boardUuid ?? ""}
                onChange={(e) => handleBoardChange(task.uuid, e.target.value)}
                disabled={saving}
              >
                <option value="">None</option>
                {boards.map((b) => (
                  <option key={b.uuid} value={b.uuid}>
                    {b.title}
                  </option>
                ))}
              </select>
            </span>
          </div>
          <div className="task-actions">
            <button
              className="secondary-btn"
              type="button"
              disabled={saving}
              onClick={() => startEdit(task)}
            >
              Edit
            </button>
            <button
              className="secondary-btn"
              type="button"
              disabled={saving}
              onClick={() => handleMarkComplete(task.uuid)}
            >
              Mark complete
            </button>
            <button
              className="secondary-btn danger"
              type="button"
              disabled={saving}
              onClick={() => onDelete(task.uuid)}
            >
              Delete
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}

