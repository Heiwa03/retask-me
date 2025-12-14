import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import TaskList from "../components/TaskList";
import TaskForm from "../components/TaskForm";
import ChatPanel from "../components/ChatPanel";
import { createTask, fetchTasks, updateTask, deleteTask } from "../api/tasks";
import { Task } from "../types";
import { useAuth } from "../hooks/useAuth";
import "../App.css";
import "../components/TaskList.css";
import "../components/TaskForm.css";
import "../components/ChatPanel.css";

export default function DashboardPage() {
  const { token, logout } = useAuth();
  const navigate = useNavigate();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const reloadTasks = async () => {
    if (!token) return;
    const data = await fetchTasks(token);
    setTasks(data);
  };

  useEffect(() => {
    if (!token) return;
    (async () => {
      try {
        await reloadTasks();
      } catch (err: any) {
        setError(err?.response?.data?.message ?? "Failed to load tasks");
      } finally {
        setLoading(false);
      }
    })();
  }, [token]);

  const handleCreateTask = async (payload: {
    title: string;
    description?: string;
    deadline?: string;
    priority: number;
    status: number;
  }) => {
    if (!token) return;
    await createTask(token, payload);
    await reloadTasks();
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const handleEditTask = async (
    taskUid: string,
    update: { title: string; description?: string; deadline?: string | null; priority: number; status: number },
  ) => {
    if (!token) return;
    await updateTask(token, taskUid, {
      title: update.title,
      description: update.description ?? "",
      deadline: update.deadline,
      priority: update.priority,
      status: update.status,
    });
    await reloadTasks();
  };

  const handleDeleteTask = async (taskUid: string) => {
    if (!token) return;
    await deleteTask(token, taskUid);
    await reloadTasks();
  };

  const handleMarkComplete = async (taskUid: string) => {
    if (!token) return;
    const task = tasks.find((t) => t.uuid === taskUid);
    if (!task) return;
    await updateTask(token, taskUid, {
      title: task.title,
      description: task.description ?? "",
      deadline: task.deadline ?? null,
      priority: task.priority,
      status: 3, // Done
    });
    await reloadTasks();
  };

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        padding: "24px",
        gap: "16px",
      }}
    >
      <header
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <div>
          <h1 style={{ margin: 0, color: "var(--purple-900)" }}>Your tasks</h1>
          <p style={{ margin: 0, color: "#5c4a73" }}>
            Organized in the center, AI on the right.
          </p>
        </div>
        <button className="secondary-btn" onClick={handleLogout}>
          Log out
        </button>
      </header>

      {error && <div className="error">{error}</div>}

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "2fr 1fr",
          gap: "18px",
          alignItems: "start",
        }}
      >
        <div className="card" style={{ display: "grid", gap: "16px" }}>
          <TaskForm onCreate={handleCreateTask} />
          {loading ? (
            <p>Loading tasks...</p>
          ) : (
            <TaskList
              tasks={tasks}
              onEdit={handleEditTask}
              onDelete={handleDeleteTask}
              onMarkComplete={handleMarkComplete}
            />
          )}
        </div>

        <ChatPanel token={token!} onAfterAgentResponse={reloadTasks} />
      </div>
    </div>
  );
}

