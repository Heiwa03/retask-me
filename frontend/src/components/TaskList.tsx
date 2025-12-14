import { Task } from "../types";
import "./TaskList.css";

interface Props {
  tasks: Task[];
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

export default function TaskList({ tasks }: Props) {
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
          <div className="task-card__meta">
            <span className={`status status--${statusLabels[task.status]?.toLowerCase() ?? "pending"}`}>
              {statusLabels[task.status] ?? "Unknown"}
            </span>
            {task.deadline && (
              <span className="deadline">
                Due {new Date(task.deadline).toLocaleDateString()}
              </span>
            )}
          </div>
        </div>
      ))}
    </div>
  );
}

