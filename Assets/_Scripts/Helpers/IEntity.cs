﻿namespace Helpers {
    
    using UnityEngine;

    using Enum;

    public interface IEntity {
        EntityType entityType { get; }
        Vector3 position { get; set; }
        Quaternion rotation { get; set; }
        bool isActive { get; set; }
    }
}