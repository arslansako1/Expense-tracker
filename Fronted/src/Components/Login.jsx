import { useState } from "react";
import { API_ENDPOINTS } from "../Configration/Urls";
import { useNavigate } from "react-router-dom";
import "../Css/Login.css";

export default function Login() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");
    setLoading(true);

    try {
      const userData = {
        Email: email,
        Password: password,
      };

      const response = await fetch(API_ENDPOINTS.login, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userData),
      });

      const text = await response.text();

      if (!response.ok) {
        setError("Invalid email or password");
        throw new Error(text || "Invalid email or password");
      }

      const data = JSON.parse(text);

      localStorage.setItem("token", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);
      localStorage.setItem(
        "user",
        JSON.stringify({
          UserId: data.userId,
          email: data.email,
          firstName: data.firstName,
          lastName: data.lastName,
          roles: data.roles,
        }),
      );

      window.location.href = "/MainPage";
    } catch (error) {
      
      setError("Invalid email or password");
    } finally {
      setLoading(false);
    }
  };

  function handleBackBtn() {
    navigate("/Intro");
  }

  return (
    <>
    <div className="login-container">
      
      <div className="login-header">
        <button className="back-btn" onClick={handleBackBtn}>Back</button>
      </div>

      <div className="login-card">
        <h1>SpendWise</h1>
        <h2>Login</h2>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              autoFocus
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div className="input-group">
            <input
              autoFocus
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button type="submit" className="login-btn" disabled={loading}>
            {" "}
            {loading ? "Logging in.." : "Login"}
          </button>
        </form>

        <p className="signup-link">
          <a href="/ForgetPassword">Forget Password?</a>
        </p>

        <p className="signup-link">
          Dont have an account? <a href="/Signup">Signup</a>
        </p>

        <p className="login-mark">© 2026 TrackWise</p>
      </div>
    </div>

    <div>
     <p className="login-footer">By using Spendee you agree with SpendWise Terms of Use, Privacy Policy</p>
    </div>
    </>
    
  );
}
