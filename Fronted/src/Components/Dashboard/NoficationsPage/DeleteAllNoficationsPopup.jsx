import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function DeleteAllNoficationPopup({onClose}){
    const dialogRef = useRef(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])

    const handleDeleteAllNofication = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            const response = await fetch(API_ENDPOINTS.deleteAllNofications,{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete all nofication"); 
                throw new Error("Failed to delete all nofication");
            }

            onClose();


        } catch(error){
            setError("Error deleting all nofication: ", error);
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="nofications-dialog" ref={dialogRef}>
      <button className="dialog-btn-no" onClick={() => onClose()}>✕</button>
        <p className="dialog-text">Are you sure you want to delete this transaction?</p>
      <button className="dialog-btn-yes" onClick={() => handleDeleteAllNofication()}>Yes</button>
    </dialog>
  );
    
}