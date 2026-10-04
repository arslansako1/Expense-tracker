import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/CreateAccPopup.css";

export default function CreateAccPopup({onClose }) {
  const dialogRef = useRef(null);
  const [name, setName] = useState("");
  const [type, setType] = useState("");
  const [currency, setCurrency] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    dialogRef.current.showModal();
  }, []);

  const handleCreateAccount = async (e) => {
    e.preventDefault();

    const token = localStorage.getItem("token");
    if (!token) {
      setLoading(false);
      setError("User must be logged in");
      return;
    }

    setError(null);

    if (!name || !type || !currency){
      setError("Fill the missing requirements");
      return;
    }

    const createAccount = {
      Name: name,
      Type: type,
      Currency: currency,
    };

    setLoading(true);

    try {
      var response = await fetch(API_ENDPOINTS.createAccount, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
        body: JSON.stringify(createAccount),
      });

      if (!response.ok) {
        setError("Failed to create account");
        throw new Error("Failed to create account");
      }

      const data = await response.json();


      onClose();
    } catch (error) {
      setError(`Error creating account: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <dialog className="create-acc-dialog" ref={dialogRef}>
        <button className="create-close-btn" onClick={onClose}>✕</button>

        {error && <div className="create-acc-error">{error}</div>}


      <form onSubmit={handleCreateAccount}>
        <label>Name</label>
        <input
          autoFocus
          type="text"
          value={name}
          onChange={(n) => setName(n.target.value)}
        />

        <label>Type</label>
        <input
          autoFocus
          type="text"
          value={type}
          onChange={(v) => setType(v.target.value)}
        />

        <label>Currency</label>
        <input
          autoFocus
          type="text"
          value={currency}
          onChange={(c) => setCurrency(c.target.value)}
        />

        <button className="create-form-button" type="submit">{loading ? "Creating..." : "Create"}</button>
      </form>

    </dialog>
  );
}
