import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function DeleteCategoryPopup({category, onClose}){
    const dialogRef = useRef(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])


    const handleDeleteCategory = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            const response = await fetch(API_ENDPOINTS.deleteCategory(category.id),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete category");
                throw new Error("Failed to delete category");
            }

            onClose();


        } catch(error){
            setError("Error deleting category: ", error);
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="delete-dialog" ref={dialogRef}>
      <button className="create-close-btn" onClick={() => onClose()}>✕</button>

        <p className="delete-text">Are you sure you want to delete this transaction?</p>
      <button className="delete-yes" onClick={() => handleDeleteCategory()}>Yes</button>
    </dialog>
  );
    
}