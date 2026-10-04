import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/EditAccPopup.css";


export default function EditBudgetPopup({budget, category, onClose}){
    const dialogRef = useRef(null);
    const [monthlyLimit, setMonthlyLimit] = useState(budget.monthlyLimit);
    const [BudgetId, setBudgetId] = useState(budget.id);
    const [categorytId, setCategoryId] = useState(category.id);
    const [month, setMonth] = useState(budget.month);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleEditBudget = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!monthlyLimit || !month){
            setError("Fill in the requirements");
            return;
        }

        const editBudget = {
           MonthlyLimit: monthlyLimit,
           CategoryId: categorytId,
           Month: month
        };

        setLoading(true);


        try{
            
            var response = await fetch(API_ENDPOINTS.updateBudget(BudgetId),{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(editBudget)
            });

            if (!response.ok){
                
                setError("Failed to edit budget");
                throw new Error("Failed to edit budget");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error editing budget: ", error);
            

        } finally{
            setLoading(false);
        }

    }
    

    return(
    <dialog className="editBudget-dialog" ref={dialogRef}>
        {error && <div className="edit-budget-error">{error}</div>}

      <button className="create-close-btn" onClick={() => onClose()}>✕</button>

      <form onSubmit={handleEditBudget}>
        <label>Monthly limit:</label>
        <input
          autoFocus
          type="number"
          value={monthlyLimit}
          onChange={(m) => setMonthlyLimit(m.target.value)}
        />

        <label>Month:</label>
        <input
          autoFocus
          type="text"
          value={month}
          onChange={(m) => setMonth(m.target.value)}
        />


        <button className="create-form-button" type="submit">{loading ? "Edting..." : "Edit"}</button>
      </form>

    </dialog>
  );
    
}