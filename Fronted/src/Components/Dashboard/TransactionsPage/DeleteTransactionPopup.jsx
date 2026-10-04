import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function DeleteTransactionPopup({transaction, account, onClose}){
    const dialogRef = useRef(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleDeleteTransaction = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            const response = await fetch(API_ENDPOINTS.deleteTransaction(transaction.id),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete transation");
                throw new Error("Failed to delete transation");
            }

            onClose();


        } catch(error){
            setError("Error deleting transation");
            

        } finally{
            setLoading(false);
        }

    }
 return(
    <dialog className="delete-dialog" ref={dialogRef}>
      <button className="delete-no" onClick={() => onClose()}>✕</button>
        <p className="delete-text">Are you sure you want to delete this transaction?</p>
      <button className="delete-yes" onClick={() => handleDeleteTransaction()}>Yes</button>
    </dialog>
  );
    
}