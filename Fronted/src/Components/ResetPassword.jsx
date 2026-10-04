import { useState, useEffect } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { API_ENDPOINTS } from "../Configration/Urls";
import "../Css/ResetPassword.css";

export default function ResetPassword() {
  const navigate = useNavigate();
  const location = useLocation();
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [token, setToken] = useState("");
  const [email, setEmail] = useState("");

  useEffect(() => {
    const parms = new URLSearchParams(location.search);
    const tokenParam = parms.get("token");
    const emailParam = parms.get("email");


    if (tokenParam && emailParam) {
      setToken(tokenParam);
      setEmail(emailParam);
    } else {
      
      setError("Invalid reset link, please request a new one");
    }
  }, [location]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    setMessage("");

    if (newPassword !== confirmPassword) {
      setError("Password do not match");
      setLoading(false);
      return;
    }

    if (newPassword.length < 6) {
      setError("Password must be atleast 6 characters");
      return;
    }

    const userData = {
      Email: email,
      NewPassword: newPassword,
      Token: token,
    };

    try {
      const response = await fetch(API_ENDPOINTS.resetPassword, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userData),
      });

      if (!response.ok) {
        setError("Reset failed");
        throw new Error("Status: ", response.status);
      }

      const data = await response.json();

      setMessage(data.message || "Password reseted successfully");

      setTimeout(() => {
        navigate("/login");
      }, 2000);

    } catch (error) {
      
      setError("Something went wrong, please try again");
      
    } finally {
      setLoading(false);
    }
  };
  if (!token || !email) {
    return (
      <div className="resetPass-container">
        <div className="resetPass-card"></div>
        <h1>Invalid link</h1>
        <p>This reset link is invalid or expired</p>
        <Link to="/forgetPassword">Request a new one</Link>
      </div>
    );
  }

  return (
    <>
    <div className="resetPass-container">
      <div className="resetPass-card">
        <h1>Reset password</h1>
        <p>Create a new password</p>

        {error && <div className="error-message">{error}</div>}

        {message && <div className="success-message">{message}</div>}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              autoFocus
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              disabled={loading}
              placeholder={"New password"}
              minLength={6}
              required
            />
          </div>

          <div className="input-group">
            <input
              autoFocus
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              disabled={loading}
              placeholder={"Confirm password"}
              minLength={6}
              required
            />
          </div>

          <button type="submit" className="resetPass-btn" disabled={loading}>
            {loading ? "Reseting..." : "Reset password"}
          </button>
        </form>

        <div className="login-link">
          <a href="/login">Login</a>
        </div>
        <p className="mark">© 2026 SkyCast</p>
          <div>
      <p className="forgetPass-footer">By using Spendee you agree with SpendWise Terms of Use, Privacy Policy</p>
    </div>
      </div>
    </div>
    </>
  );
}
