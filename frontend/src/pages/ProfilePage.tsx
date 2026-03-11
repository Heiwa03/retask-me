import { FormEvent, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getProfile, registerProfile, updateProfile } from "../api/profile";
import { Gender, UserProfile } from "../types";
import { useAuth } from "../hooks/useAuth";
import "../App.css";

const genderOptions: Array<{ value: Gender; label: string }> = [
  { value: 0, label: "Female" }, // backend enum name is Girl
  { value: 1, label: "Male" },
  { value: 2, label: "Unspecified" },
];

export default function ProfilePage() {
  const { token, logout } = useAuth();
  const navigate = useNavigate();
  const [profile, setProfile] = useState<UserProfile>({
    firstName: "",
    lastName: "",
    gender: 2,
  });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isExisting, setIsExisting] = useState(false);

  useEffect(() => {
    if (!token) return;
    (async () => {
      try {
        const data = await getProfile(token);
        if (data) {
          setProfile(data);
          setIsExisting(true);
        }
      } catch (err: any) {
        setError(err?.response?.data?.message ?? "Failed to load profile");
      } finally {
        setLoading(false);
      }
    })();
  }, [token]);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!token) return;
    setSaving(true);
    setError(null);
    try {
      if (isExisting) {
        await updateProfile(token, profile);
      } else {
        await registerProfile(token, profile);
        setIsExisting(true);
      }
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Failed to save profile");
    } finally {
      setSaving(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="page">
      <div className="card" style={{ width: "520px", display: "grid", gap: 16 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <div>
            <h2 style={{ margin: 0 }} className="purple-text">
              Your profile
            </h2>
            <p style={{ margin: "4px 0 0", color: "#5c4a73" }}>
              Manage your name and gender.
            </p>
          </div>
          <button className="secondary-btn" onClick={handleLogout}>
            Log out
          </button>
        </div>

        <div style={{ display: "flex", gap: 12 }}>
          <Link to="/dashboard" className="purple-text">
            ← Back to dashboard
          </Link>
        </div>

        {error && <div className="error">{error}</div>}
        {loading ? (
          <p>Loading profile...</p>
        ) : (
          <form className="form-grid" onSubmit={handleSubmit}>
            <div>
              <label className="label">First name</label>
              <input
                className="input"
                required
                value={profile.firstName}
                onChange={(e) => setProfile((p) => ({ ...p, firstName: e.target.value }))}
              />
            </div>
            <div>
              <label className="label">Last name</label>
              <input
                className="input"
                required
                value={profile.lastName}
                onChange={(e) => setProfile((p) => ({ ...p, lastName: e.target.value }))}
              />
            </div>
            <div>
              <label className="label">Gender</label>
              <select
                className="input"
                value={profile.gender}
                onChange={(e) => setProfile((p) => ({ ...p, gender: Number(e.target.value) as Gender }))}
              >
                {genderOptions.map((g) => (
                  <option key={g.value} value={g.value}>
                    {g.label}
                  </option>
                ))}
              </select>
            </div>

            <button className="primary-btn" type="submit" disabled={saving}>
              {saving ? "Saving..." : isExisting ? "Save changes" : "Create profile"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}

