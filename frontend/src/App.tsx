import { Navigate, Route, Routes } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import DashboardPage from "./pages/DashboardPage";
import ProfilePage from "./pages/ProfilePage";
import WelcomePage from "./pages/WelcomePage";
import { useAuth } from "./hooks/useAuth";
import { useTheme } from "./context/ThemeContext";

function ProtectedRoute({ children }: { children: JSX.Element }) {
  const { token, isReady } = useAuth();

  if (!isReady) {
    // Prevent flicker/redirect until we know auth state from localStorage
    return null;
  }
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return children;
}

export default function App() {
  const { token, isReady } = useAuth();
  const { isDark, toggleTheme } = useTheme();
  const seenVideo = typeof window !== "undefined" && localStorage.getItem("retask-video-seen") === "1";

  if (!isReady) {
    return null;
  }

  return (
    <>
      <button
        aria-label="Toggle theme"
        onClick={toggleTheme}
        style={{
          position: "fixed",
          bottom: 16,
          right: 16,
          zIndex: 1000,
          padding: "10px 14px",
          borderRadius: "999px",
          border: "1px solid var(--neutral-400)",
          background: "var(--card-bg)",
          color: "var(--text)",
          boxShadow: "var(--shadow)",
          cursor: "pointer",
        }}
      >
        {isDark ? "Light mode" : "Dark mode"}
      </button>
      <Routes>
        <Route
          path="/"
          element={<Navigate to={token ? (seenVideo ? "/dashboard" : "/welcome") : "/login"} replace />}
        />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/welcome"
          element={
            <ProtectedRoute>
              <WelcomePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <DashboardPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </>
  );
}

