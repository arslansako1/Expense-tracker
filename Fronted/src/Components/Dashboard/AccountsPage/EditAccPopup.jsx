import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/EditAccPopup.css";



export default function EditAccPopup({account, onClose}){
    const dialogRef = useRef(null);
    const [name, setName] = useState(account.name);
    const [type, setType] = useState(account.type);
    const [currency, setCurrency] = useState(account.currency);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleEditAccount = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!name || !type || !currency){
          setError("Fill the missing requirements");
          return;
        }

        const editAccount = {
            Name: name,
            Type: type,
            Currency: currency     
        };

        setLoading(true);

        try{
            var response = await fetch(API_ENDPOINTS.updateAccount(account.id),{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(editAccount)
            });

            if (!response.ok){
                
                setError("Failed to edit account");
                throw new Error("Failed to edit account");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error edit account");
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="editAcc-dialog" ref={dialogRef}>
      <button className="edit-close-btn" onClick={() => onClose()}>✕</button>
      
        {error && <div className="create-acc-error">{error}</div>}

      <form onSubmit={handleEditAccount}>
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

        <button className="edit-form-button" type="submit">{loading ? "Edting..." : "Edit"}</button>
      </form>

    </dialog>
  );
    
}