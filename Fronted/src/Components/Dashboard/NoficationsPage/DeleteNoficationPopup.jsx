import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";


export default function DeleteNoficationPopup({nofication, onClose}){
    const dialogRef = useRef(null);
    const [noficationId, setNoficationId] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setNoficationId(nofication.id);

    }, [])


    const handleDeleteNofication = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            const response = await fetch(API_ENDPOINTS.deleteNofication(noficationId),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete nofication");
                throw new Error("Failed to delete nofication");
            }

            onClose();


        } catch(error){
            setError("Error deleting nofication: ", error);
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog ref={dialogRef}>
        <p>Are you sure you want to delete this transaction?</p>
      <button onClick={() => handleDeleteNofication()}>Yes</button>
      <button onClick={() => onClose()}>Close</button>
    </dialog>
  );
    
}