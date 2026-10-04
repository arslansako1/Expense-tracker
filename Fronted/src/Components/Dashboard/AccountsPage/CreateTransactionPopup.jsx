import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/CreateTransactionPopup.css";


export default function CreateTransactionPopup({accountId, onClose}){
    const dialogRef = useRef(null);
    const [amount, setAmount] = useState(0);
    const [type, setType] = useState("Income");
    const [description, setDescription] = useState("");
    const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
    const [categoryId, setCategoryId] = useState(0);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(false);
    const [loadingCategories, setLoadingCategories] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        fetchAllCategories();

    }, [])


    const handleCreateTransaction = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        if (!amount || !type || !description || !categoryId){
           setError("Fill the missing requirements");
          return;
        }

        setError(null);

        const createTransaction = {
            AccountId: accountId,
            Amount: amount,
            Type: type,
            Description: description,
            CategoryId: parseInt(categoryId),
            Date: new Date(date).toISOString(),
        };

        setLoading(true);

        try{
            var response = await fetch(API_ENDPOINTS.createTransaction,{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(createTransaction)
            });

            if (!response.ok){
                const errorTxt = await response.text();
                setError(errorTxt || "Failed to create transaction");
                throw new Error(errorTxt || "Failed to create transaction");
            }

            const data = await response.json();
            
            onClose();


        } catch(error){
            setError(error.message || "Error creating transaction");

        } finally{
            setLoading(false);
        }

    }
    
        const fetchAllCategories = async (e) => {
            const token = localStorage.getItem("token");
            if (!token){
              setLoading(false);
              return;
            }
      
          setLoadingCategories(true);
          setError(null);
      
      
          try{
            const response = await fetch(API_ENDPOINTS.getAllCategories,{
              headers: {
                Authorization: "Bearer " + token, 
              },
            });
      
            if (!response.ok){
              setError("Failed to fetch all categories");
              throw new Error("Failed to fetch all categories");
            }
      
            const data = await response.json();
            setCategories(data);
      
          } catch(error){
            setError(`Error fetching all categories: ${error.message}`);
      
          } finally{
           setLoadingCategories(false);
          }
        }


    return(
        <dialog className="transaction-dialog" ref={dialogRef}>
            <button type="button" className="transaction-close-button" onClick={() => onClose()}>✕</button>

            <form onSubmit={handleCreateTransaction}>
               {error && <div className="transaction-error">{error}</div>}
 

                <label className="amount">Amount</label>
                <input autoFocus className="amount-input" type="number" value={amount} onChange={(a) => setAmount(parseFloat(a.target.value))}/>

                <label className="type">Type</label>
                <select value={type} onChange={(t) => setType(t.target.value)}>
                <option value={"Income"}>Income</option>
                <option value={"Expense"}>Expense</option>
                </select>

                <label className="description">description</label>
                <input autoFocus className="description-input" type="text" value={description} onChange={(d) => setDescription(d.target.value)}/>

                <label className="category">Category</label>
                <select value={categoryId} onChange={(c) => setCategoryId(parseInt(c.target.value))}>
                    <option value="">Select a category</option> 
                    {categories.map((category) => (
                        <option value={category.id} key={category.id}>{`${category.name} ${category.icon}`}</option>
                    ))}     
                </select>
            <button className="transaction-create-button" type="submit">{loading ? "Creating..." : "Create"}</button>


            </form>

        </dialog>

    )
}