import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/AccDeletePopup.css";


export default function AccDeletePopup({account, onClose}){
    const dialogRef = useRef(null);
    const [accountName, setAccountName] = useState(account.name);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleDeleteAccount = async (e) => {

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   

        try{
            const response = await fetch(API_ENDPOINTS.deleteAccount(account.id),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                setError("Failed to delete account");
                throw new Error("Failed to delete account");
            }

            onClose();


        } catch(error){
            setError(`Error deleting account: ${error.message}`);

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="delete-dialog" ref={dialogRef}>
      <button className="delete-no" onClick={() => onClose()}>✕</button>
        <p className="delete-text">Are you sure you want to delete this account?</p>
        <p className="delete-name">{accountName}</p>
      <button className="delete-yes" onClick={() => handleDeleteAccount()}>Yes</button>
    </dialog>
  );
    
}