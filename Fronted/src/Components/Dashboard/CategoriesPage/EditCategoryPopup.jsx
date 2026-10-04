import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/CategoryPage/Category.css";


export default function EditCategoryPopup({category, onClose}){
    const dialogRef = useRef(null);
    const [name, setName] = useState("");
    const [icon, setIcon] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setName(category.name);
        setIcon(category.icon);

    }, [])


    const handleEditCategory = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!name){
            setError("fill in the missing requirements");
            return;
        }

        const editCategory = {
           Name: name,
           Icon: icon
        };

        setLoading(true);


        try{
            var response = await fetch(API_ENDPOINTS.updateCategory(category.id),{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(editCategory)
            });

            if (!response.ok){
                
                setError("Failed to edit category");
                throw new Error("Failed to edit category");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error editing category: ", error);
            

        } finally{
            setLoading(false);
        }

    }
    

    return(
    <dialog className="editCategory-dialog" ref={dialogRef}>

      <button className="create-close-btn" onClick={() => onClose()}>✕</button>

      <form onSubmit={handleEditCategory}>
            {error && <div className="create-budget-error">{error}</div>}


        <label>Name</label>
        <input
          autoFocus
          type="text"
          value={name}
          onChange={(n) => setName(n.target.value)}
        />

        <label>Icon</label>
        <input
          autoFocus
          type="text"
          value={icon}
          onChange={(i) => setIcon(i.target.value)}
        />

        <button className="create-form-button" type="submit">{loading ? "Edting..." : "Edit"}</button>
      </form>

    </dialog>
  );
    
}