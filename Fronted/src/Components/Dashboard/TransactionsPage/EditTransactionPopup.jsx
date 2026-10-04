import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function EditTransactionPopup({categories, transaction, account, onClose}){
    const dialogRef = useRef(null);
    const [Amount, setAmount] = useState(transaction.amount);
    const [type, setType] = useState(transaction.type);
    const [description, setDescription] = useState(transaction.description);
    const [categoryId, setCategoryId] = useState(transaction.categoryId);
    const [allCategories, setAllCategories] = useState(categories);
    const [loadingCategory, setLoadingCategory] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleEditTransaction = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

          if (!Amount || !type || !description || !categoryId){
            setError("Fill the missing requirements");
            return;
        }

        setError(null);

        const editTransaction = {
            Amount: Amount,
            Type: type,
            Description: description,  
            CategoryId: categoryId
        };

        

        setLoading(true);


        try{
            var response = await fetch(API_ENDPOINTS.updateTransaction(transaction.id),{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(editTransaction)
            });

            if (!response.ok){
                
                setError("Failed to edit transaction");
                throw new Error("Failed to edit transaction");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error editing transaction");
            

        } finally{
            setLoading(false);
        }

    }

   
    
return(
    <dialog className="editTransaction-dialog" ref={dialogRef}>

      <button className="create-close-btn" onClick={() => onClose()}>✕</button>

        {error && <div className="create-acc-error">{error}</div>}

      <form onSubmit={handleEditTransaction}>
        <label>Amount</label>
        <input
          autoFocus
          type="text"
          value={Amount}
          onChange={(a) => setAmount(a.target.value)}
        />

        <label>Type</label>
        <input
          autoFocus
          type="text"
          value={type}
          onChange={(t) => setType(t.target.value)}
        />

        <label>Description</label>
        <input
          autoFocus
          type="text"
          value={description}
          onChange={(d) => setDescription(d.target.value)}
        />

         <label>Category</label>
                <select value={categoryId} onChange={(c) => setCategoryId(parseInt(c.target.value))}>
                    <option value="">Select a category</option> 
                    {allCategories.map((category) => (
                        <option value={category.id} key={category.id}>{`${category.name} ${category.icon}`}</option>
                    ))}     
                </select>

        <button className="edit-form-button" type="submit">{loading ? "Edting..." : "Edit"}</button>
      </form>

    </dialog>
  );
    
    
}