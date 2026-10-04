import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useViewTransitionState } from "react-router-dom";
import "/src/Css/Dashboard/BudgetPage/Budgets.css";

export default function CreateBudgetPopup({category, onClose}){
    const dialogRef = useRef(null);
    const [monthlyLimit, setMonthlyLimit] = useState(0);
    const [categoryId, setCategoryId] = useState(0);
    const [month, setMonth] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setCategoryId(category.id);


    }, [])


    const handleCreateBudget = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!monthlyLimit || !categoryId || !month){
            setError("Fill the missing requirements");
            return;
        }

        const createBudget = {
            MonthlyLimit: monthlyLimit,
            CategoryId: categoryId,
            Month: month
        };

        setLoading(true);

        try{
            var response = await fetch(API_ENDPOINTS.createBudget,{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(createBudget)
            });

            if (!response.ok){
                
                setError("Failed to create budget");
                throw new Error("Failed to create budget");
            }

            const data = await response.json();
            

            onClose();

        } catch(error){
            setError("Error creating budget");
            

        } finally{
            setLoading(false);
        }

    }

    return(
        <dialog className="createBudget-dialog" ref={dialogRef}>
            <form onSubmit={handleCreateBudget}>
        {error && <div className="create-budget-error">{error}</div>}

            <button className="create-close-btn" onClick={() => onClose()}>✕</button>

                <label>Monthly limit:</label>
                <input autoFocus type="number" value={monthlyLimit} onChange={(m) => setMonthlyLimit(m.target.value)}/>

                <label>Month:</label>
                <input autoFocus type="text" value={month} onChange={(m) => setMonth(m.target.value)}/>

            <button className="create-form-button" type="submit">{loading ? "Creating..." : "Create"}</button>
            </form>

        </dialog>


    )
}