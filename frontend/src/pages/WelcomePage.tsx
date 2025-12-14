import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import "../App.css";

const VIDEO_FLAG = "retask-video-seen";

export default function WelcomePage() {
  const navigate = useNavigate();
  const { logout } = useAuth();

  const handleContinue = () => {
    localStorage.setItem(VIDEO_FLAG, "1");
    navigate("/dashboard");
  };

  return (
    <div className="page">
      <div className="card" style={{ maxWidth: 720, width: "100%", display: "grid", gap: 16 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <div>
            <h2 style={{ margin: 0, color: "var(--purple-900)" }}>Welcome to ReTask-Me</h2>
            <p style={{ margin: "4px 0 0", color: "var(--text)" }}>
              A quick intro before you dive into your tasks.
            </p>
          </div>
          <button className="secondary-btn" onClick={logout}>
            Log out
          </button>
        </div>
        <div style={{ position: "relative", paddingTop: "56.25%", borderRadius: 12, overflow: "hidden", boxShadow: "var(--shadow)" }}>
          <iframe
            title="ReTask-Me intro"
            src="https://www.youtube.com/embed/92P3YU0zSdk"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
            allowFullScreen
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              width: "100%",
              height: "100%",
              border: "none",
            }}
          />
        </div>
        <div style={{ display: "flex", justifyContent: "flex-end", gap: 12 }}>
          <button className="secondary-btn" onClick={logout}>
            Log out
          </button>
          <button className="primary-btn" onClick={handleContinue}>
            Continue to tasks
          </button>
        </div>
      </div>
    </div>
  );
}

