import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function DeletebudgetPopup({budget, onClose}){
    const dialogRef = useRef(null);
    const [budgetId, setBudgetId] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setBudgetId(budget.id);

    }, [])


    const handleDeleteBudget = async (e) => {
        
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            
            const response = await fetch(API_ENDPOINTS.deleteBudget(budgetId),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete budget");
                throw new Error("Failed to delete budget");
            }

            

            onClose();


        } catch(error){
            setError("Error deleting budget: ", error);
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="delete-dialog" ref={dialogRef}>
        <button className="create-close-btn" onClick={() => onClose()}>✕</button>
        <p className="delete-text">Are you sure you want to delete this budget?</p>
      <button className="delete-yes" onClick={() => handleDeleteBudget()}>Yes</button>
    </dialog>
  );
    
}