import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../Configration/Urls";
import "../Css/Signup.css";

export default function Signup() {
  const navigate = useNavigate();
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    setError("");
    setSuccess(false);

    if (password !== confirmPassword) {
      setError("Password does not match");
      return;
    }

    setLoading(true);

    try {
      const userData = {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Password: password,
        ConfirmPassword: confirmPassword,
      };


      const response = await fetch(API_ENDPOINTS.signup, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userData),
      });

      const text = await response.text();

      if (!response.ok) {
        try {
          const textData = JSON.parse(text);

          if (textData.errors) {
            const errorMessages = [];
            for (const [field, message] of Object.entries(textData.errors)) {
              errorMessages.push(`${field}: ${message.join(", ")}`);
            }
            setError(errorMessages.join("\n"));
            throw new Error(errorMessages.join("\n"));
          } else {
            setError(textData.message || "Signup failed");
            throw new Error(textData.message || "Signup failed");
          }
        } catch {
          setError(text || "Signup failed");
          throw new Error(text || "Signup failed");
        }
      }

      const data = JSON.parse(text);

      setSuccess(true);
      
      setTimeout(() => {
        navigate("/login");
      }, 2000);
    } catch (error) {
      

      if (!error) {
        setError(`Signup failed: ${error.message}`);
      }
    } finally {
      setLoading(false);
    }
  };

    function handleBackBtn(){
      navigate("/Intro");
    }

  return (
    <>
    <div className="signup-container">

      <div className="signup-header">
        <button className="back-btn" onClick={handleBackBtn}>Back</button>
      </div>

      <div className="signup-card">
        <h1>SpendWise</h1>
        <h2>Signup</h2>

        {error && <div className="error-message">{error}</div>}
        {success && (
          <div className="success-message">Success! account created</div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              autoFocus
              type="text"
              placeholder="First Name"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
            />
          </div>

          <div className="input-group">
            <input
              autoFocus
              type="text"
              placeholder="Last Name"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
            />
          </div>

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

          <div className="input-group">
            <input
              autoFocus
              type="password"
              placeholder="Confirm passowrd"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
            />
          </div>

          <button
            type="submit"
            className="signup-btn"
            disabled={loading || success}
          >
            {" "}
            {loading ? "Creating account" : "Signup"}{" "}
          </button>
        </form>

        <div className="login-link">
          Already have account? <a href="/Login">Login</a>
        </div>

        <div className="login-link"></div>
        <p className="login-mark">© 2026 SpendWise</p>
      </div>
    </div>

    <div>
     <p className="signup-footer">By using Spendee you agree with SpendWise Terms of Use, Privacy Policy</p>
    </div>
    </>
  );
}
